using Infrastructure.CurrencyRatesService;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Frozen;
using System.Net.Http.Json;

namespace Infrastructure.CurrencyRatesService;


public sealed class CurrencyExchangeService : ICurrencyExchangeService, IHostedService, IAsyncDisposable
{

    private readonly HttpClient _http;
    private readonly IOptions<CurrencyExchangeOptions> _options;
    private readonly ILogger<CurrencyExchangeService> _logger;

    private volatile FrozenDictionary<string, FrozenDictionary<string, decimal>>? _rates;
    private DateTimeOffset? _lastRefreshedAt;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private Timer? _backgroundTimer;
    private CancellationTokenSource? _cts;

    public DateTimeOffset? LastRefreshedAt => _lastRefreshedAt;


    public CurrencyExchangeService(
        HttpClient http,
        IOptions<CurrencyExchangeOptions> options,
        ILogger<CurrencyExchangeService> logger)
    {
        _http = http;
        _options = options;
        _logger = logger;
    }

   
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        await RefreshRatesAsync(_cts.Token);

        var interval = _options.Value.RefreshInterval;
        _backgroundTimer = new Timer(
            callback: _ => _ = BackgroundRefreshAsync(),
            state: null,
            dueTime: interval,
            period: interval);

        _logger.LogInformation("CurrencyExchangeService started. Background refresh every {Interval}.", interval);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts?.Cancel();
        _backgroundTimer?.Change(Timeout.Infinite, Timeout.Infinite);
        _logger.LogInformation("CurrencyExchangeService stopped.");
        return Task.CompletedTask;
    }

    public IReadOnlyDictionary<string, IReadOnlyDictionary<string, decimal>> GetCachedRates()
    {
        var snapshot = _rates;
        if (snapshot is null)
            return FrozenDictionary<string, IReadOnlyDictionary<string, decimal>>.Empty;

        return snapshot.ToFrozenDictionary(
            kvp => kvp.Key,
            kvp => (IReadOnlyDictionary<string, decimal>)kvp.Value);
    }


    public async Task<decimal> GetRateAsync(string from, string to, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(from);
        ArgumentException.ThrowIfNullOrWhiteSpace(to);

        from = from.ToUpperInvariant();
        to = to.ToUpperInvariant();

        if (from == to) return 1m;


        if (IsStale())
        {
            _logger.LogDebug("Rates are stale or missing. Triggering synchronous refresh before serving {From}/{To}.", from, to);
            await RefreshRatesAsync(ct);
        }

        var rates = _rates
            ?? throw new InvalidOperationException("Exchange rates could not be loaded.");

        return TryGetRate(rates, from, to)
            ?? throw new KeyNotFoundException(
                $"No exchange rate found for {from} → {to}. " +
                $"Available bases: {string.Join(", ", rates.Keys)}");
    }

    
    private static decimal? TryGetRate(
        FrozenDictionary<string, FrozenDictionary<string, decimal>> rates,
        string from,
        string to)
    {
        if (rates.TryGetValue(from, out var fromRates) && fromRates.TryGetValue(to, out var direct))
            return direct;

        if (rates.TryGetValue(to, out var toRates) && toRates.TryGetValue(from, out var inverse))
            return 1m / inverse;

        return null;
    }

    
    private bool IsStale()
    {
        if (_rates is null || _lastRefreshedAt is null)
            return true;

        return DateTimeOffset.UtcNow - _lastRefreshedAt.Value > _options.Value.MaxDataAge;
    }

    
    private async Task BackgroundRefreshAsync()
    {
        await RefreshRatesAsync(_cts?.Token ?? CancellationToken.None);
    }

    private async Task RefreshRatesAsync(CancellationToken ct)
    {
        await _refreshLock.WaitAsync(ct);
        try
        {
            if (!IsStale()) return;

            _logger.LogInformation("Refreshing exchange rates from exchangeratesapi.io…");

            var opts = _options.Value;

            var url = $"{opts.BaseUrl}latest?access_key={opts.ApiKey}";
            var response = await _http.GetFromJsonAsync<ExchangeRatesApiResponse>(url, ct);

            if (response is null || !response.Success)
            {
                var info = response?.Error?.Info ?? "unknown error"; _logger.LogError("exchangeratesapi returned failure: {Info}", info);
                return;
            }

            _logger.LogDebug("Received {Count} rates with base {Base} for {Date}.", response.Rates.Count, response.Base, response.Date);

            var builder = BuildRateTable(response);

            _rates = builder.ToFrozenDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase),
                StringComparer.OrdinalIgnoreCase);

            _lastRefreshedAt = DateTimeOffset.UtcNow;

            _logger.LogInformation("Exchange rates refreshed successfully at {Time}. Bases: {Bases}.", _lastRefreshedAt, string.Join(", ", _rates.Keys));
        }
        finally
        {
            _refreshLock.Release();
        }
    }


    private Dictionary<string, Dictionary<string, decimal>> BuildRateTable(ExchangeRatesApiResponse response)
    {
        var opts = _options.Value;
        var eurRates = response.Rates;
        var apiBase = response.Base;
        var result = new Dictionary<string, Dictionary<string, decimal>>(StringComparer.OrdinalIgnoreCase);

        result[apiBase] = new Dictionary<string, decimal>(eurRates, StringComparer.OrdinalIgnoreCase);

        result[apiBase][apiBase] = 1m;

        foreach (var wantedBase in opts.BaseCurrencies)
        {
            if (string.Equals(wantedBase, apiBase, StringComparison.OrdinalIgnoreCase))
                continue;

            if (!eurRates.TryGetValue(wantedBase, out var wantedInEur) || wantedInEur == 0m)
            {
                _logger.LogWarning(
                    "Cannot derive base {Base}: rate not found in API response.", wantedBase);
                continue;
            }

            var derived = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
            {
                [wantedBase] = 1m
            };

            foreach (var (target, rateFromEur) in eurRates)
            {
                derived[target] = rateFromEur / wantedInEur;
            }

            result[wantedBase] = derived;
        }

        return result;
    }


    public async ValueTask DisposeAsync()
    {
        if (_backgroundTimer is not null)
            await _backgroundTimer.DisposeAsync();

        _refreshLock.Dispose();
        _cts?.Dispose();
    }
}
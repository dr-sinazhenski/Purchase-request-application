namespace Infrastructure.CurrencyRatesService;

public interface ICurrencyExchangeService
{
    Task<decimal> GetRateAsync(string from, string to, CancellationToken ct = default);

    IReadOnlyDictionary<string, IReadOnlyDictionary<string, decimal>> GetCachedRates();

    DateTimeOffset? LastRefreshedAt { get; }
}
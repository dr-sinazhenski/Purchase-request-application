using Infrastructure.CurrencyRatesService;
using System;
using System.Collections.Generic;
using System.Text;

namespace Testing
{
    public sealed class FakeCurrencyExchangeService : ICurrencyExchangeService
    {
        public DateTimeOffset? LastRefreshedAt => DateTimeOffset.UtcNow;

        public Task<decimal> GetRateAsync(string from, string to, CancellationToken ct = default)
            => Task.FromResult(from == to ? 1m : 1m);

        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, decimal>> GetCachedRates()
            => new Dictionary<string, IReadOnlyDictionary<string, decimal>>();
    }
}

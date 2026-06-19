namespace Infrastructure.CurrencyRatesService;

public sealed class CurrencyExchangeOptions
{
    public string ApiKey { get; set; }
    public string BaseUrl { get; set; }
    public TimeSpan RefreshInterval { get; set; }
    public TimeSpan MaxDataAge { get; set; }
    public IList<string> BaseCurrencies { get; set; }
}
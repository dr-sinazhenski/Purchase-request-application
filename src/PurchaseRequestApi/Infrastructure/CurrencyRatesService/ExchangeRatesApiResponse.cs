using System.Text.Json.Serialization;

namespace Infrastructure.CurrencyRatesService;

public sealed class ExchangeRatesApiResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("base")]
    public string Base { get; init; } = "EUR";

    [JsonPropertyName("date")]
    public string Date { get; init; } = string.Empty;

    [JsonPropertyName("rates")]
    public Dictionary<string, decimal> Rates { get; init; } = [];

    [JsonPropertyName("error")]
    public ApiError? Error { get; init; }
}

public sealed class ApiError
{
    [JsonPropertyName("code")]
    public int Code { get; init; }

    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    [JsonPropertyName("info")]
    public string Info { get; init; } = string.Empty;
}
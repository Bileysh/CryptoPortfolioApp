using System.Text.Json.Serialization;
using CryptoApp.Services.Models.Dtos;

namespace CryptoApp.Services.Models.Response;

public class CoinGeckoTickersResponse
{
    [JsonPropertyName("tickers")]
    public List<CoinGeckoTickerDto> Tickers { get; set; } = new();
}
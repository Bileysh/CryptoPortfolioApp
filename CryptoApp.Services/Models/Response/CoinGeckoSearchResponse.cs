using System.Text.Json.Serialization;
using CryptoApp.Services.Models.Dtos;

namespace CryptoApp.Services.Models.Response;

public class CoinGeckoSearchResponse
{
    [JsonPropertyName("coins")]
    public List<CoinGeckoSearchCoinDto> Coins { get; set; } = new();
}
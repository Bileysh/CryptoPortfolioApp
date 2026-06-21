using System.Text.Json.Serialization;

namespace CryptoApp.Services.Models.Dtos;

public class CoinGeckoSearchCoinDto
{
    [JsonPropertyName("id")] 
    public string Id { get; set; } = string.Empty;
}
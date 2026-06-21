using System.Text.Json.Serialization;

namespace CryptoApp.Services.Models.Dtos;

public class CoinGeckoMarketInfoDto
{
    [JsonPropertyName("name")] 
    public string Name { get; set; } = string.Empty;
}
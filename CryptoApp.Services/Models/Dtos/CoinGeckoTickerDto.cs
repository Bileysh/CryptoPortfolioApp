using System.Text.Json.Serialization;

namespace CryptoApp.Services.Models.Dtos;

public class CoinGeckoTickerDto
{
    [JsonPropertyName("base")] 
    public string? Base { get; set; } = string.Empty;
    [JsonPropertyName("target")]
    public string? Target { get; set; } = string.Empty;
    [JsonPropertyName("last")]
    public decimal? Last { get; set; }
    [JsonPropertyName("trade_url")]
    public string? TradeUrl { get; set; } = string.Empty;
    [JsonPropertyName("market")] 
    public CoinGeckoMarketInfoDto? Market { get; set; } = new();
}
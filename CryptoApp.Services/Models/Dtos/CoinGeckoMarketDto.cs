using System.Text.Json.Serialization;

namespace CryptoApp.Services.Models.Dtos;

public class CoinGeckoMarketDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("current_price")]
    public decimal? CurrentPrice { get; set; }

    [JsonPropertyName("price_change_percentage_24h")]
    public decimal? PriceChangePercentage24H { get; set; }

    [JsonPropertyName("market_cap")]
    public decimal? MarketCap { get; set; }

    [JsonPropertyName("total_volume")]
    public decimal? TotalVolume { get; set; }
}
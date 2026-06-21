namespace CryptoApp.Models.Entities;

public class MarketInfo
{
    public string ExchangeName { get; set; } = string.Empty;
    public string Pair { get; set; } = string.Empty;
    public decimal PriceUsd { get; set; }
    public string TradeUrl { get; set; } = string.Empty;
}
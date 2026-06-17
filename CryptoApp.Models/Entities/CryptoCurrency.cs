namespace CryptoApp.Models.Entities;

public class CryptoCurrency
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public decimal PriceUsd { get; set; }
    public decimal ChangePercent24Hr { get; set; }
    public decimal MarketCap { get; set; }
    public decimal Volume24Hr { get; set; }
}
namespace CryptoApp.Services.Models;

public class CryptoApiOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.coingecko.com/api/v3";
}
using System.Text.Json;
using CryptoApp.Models.Entities;
using CryptoApp.Services.Interfaces;
using CryptoApp.Services.Models.Dtos;
using CryptoApp.Services.Models.Response;
using Microsoft.Extensions.Configuration;

namespace CryptoApp.Services.Implementations;

public class CryptoApiService: ICryptoApiService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://api.coingecko.com/api/v3";
    
    public CryptoApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "CryptoPortfolioApp/1.0");
        
        var apiKey = configuration["CoinGeckoApiKey"];
        
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            _httpClient.DefaultRequestHeaders.Add("x-cg-demo-api-key", apiKey);
        }
        
    }

    public async Task<IEnumerable<CryptoCurrency>> GetTopCurrenciesAsync(int count = 50, CancellationToken cancellationToken = default)
    {
        var url = $"{BaseUrl}/coins/markets?vs_currency=usd&order=market_cap_desc&per_page={count}&page=1&sparkline=false";
        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var dtos = JsonSerializer.Deserialize<List<CoinGeckoMarketDto>>(content) 
                       ?? new List<CoinGeckoMarketDto>();
            
            return dtos.Select(dto => new CryptoCurrency
            {
                Id = dto.Id,
                Name = dto.Name,
                Symbol = dto.Symbol.ToUpper(), 
                PriceUsd = dto.CurrentPrice,
                ChangePercent24Hr = dto.PriceChangePercentage24H,
                MarketCap = dto.MarketCap,
                Volume24Hr = dto.TotalVolume
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching top currencies: {ex.Message}");
            return [];
        }
    }
    public async Task<IEnumerable<CryptoCurrency>> SearchCurrenciesAsync(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(query))
            return [];
        
        var url = $"{BaseUrl}/search?query={Uri.EscapeDataString(query)}";
        var searchResponse = await _httpClient.GetAsync(url, cancellationToken);
        searchResponse.EnsureSuccessStatusCode();
        
        var searcgContent = await searchResponse.Content.ReadAsStringAsync(cancellationToken);
        var searchResult = JsonSerializer.Deserialize<CoinGeckoSearchResponse>(searcgContent);
        
        if (searchResult?.Coins == null || !searchResult.Coins.Any())
            return [];
        
        var ids = string.Join(",", searchResult.Coins.Take(5).Select(c => c.Id));
        
        var marketsUrl = $"{BaseUrl}/coins/markets?vs_currency=usd&ids={ids}";
        var marketsResponse = await _httpClient.GetAsync(marketsUrl, cancellationToken);
        marketsResponse.EnsureSuccessStatusCode();
        
        var marketContent = await marketsResponse.Content.ReadAsStringAsync(cancellationToken);
        var dtos = JsonSerializer.Deserialize<List<CoinGeckoMarketDto>>(marketContent)
                   ?? new List<CoinGeckoMarketDto>();

        return dtos.Select(dto => new CryptoCurrency
        {
            Id = dto.Id,
            Name = dto.Name,
            Symbol = dto.Symbol.ToUpper(),
            PriceUsd = dto.CurrentPrice,
            ChangePercent24Hr = dto.PriceChangePercentage24H,
            MarketCap = dto.MarketCap,
            Volume24Hr = dto.TotalVolume
        });
    }

    public async Task<CryptoCurrency?> GetCurrencyDetailsAsync(string id, CancellationToken cancellationToken = default)
    {
        var url = $"{BaseUrl}/coins/markets?vs_currency=usd&ids={Uri.EscapeDataString(id)}";
        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var dtos = JsonSerializer.Deserialize<List<CoinGeckoMarketDto>>(content)
                   ?? new List<CoinGeckoMarketDto>();
        
        var dto = dtos.FirstOrDefault();
        if (dto == null) return null;
        
        return new CryptoCurrency
        {
            Id = dto.Id,
            Name = dto.Name,
            Symbol = dto.Symbol.ToUpper(),
            PriceUsd = dto.CurrentPrice,
            ChangePercent24Hr = dto.PriceChangePercentage24H,
            MarketCap = dto.MarketCap,
            Volume24Hr = dto.TotalVolume
        };
    }

    public async Task<IEnumerable<MarketInfo>> GetMarketsForCurrencyAsync(string id, CancellationToken cancellationToken = default)
    {
        var url = $"{BaseUrl}/coins/{Uri.EscapeDataString(id)}/tickers?include_exchange_logo=false&order=volume_desc";
        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<CoinGeckoTickersResponse>(content);
        
        if (result?.Tickers == null)
            return [];
        
        var tickers = result.Tickers
            .Where(t => !string.IsNullOrEmpty(t.TradeUrl))
            .Take(10)
            .Select(t => new MarketInfo
            {
                ExchangeName = t.Market.Name,
                Pair = $"{t.Base}/{t.Target}",
                PriceUsd = t.Last,
                TradeUrl = t.TradeUrl
            });

        return tickers;
    }
}
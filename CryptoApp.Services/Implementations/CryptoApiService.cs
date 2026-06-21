using System.Text.Json;
using CryptoApp.Models.Entities;
using CryptoApp.Services.Interfaces;
using CryptoApp.Services.Models;
using CryptoApp.Services.Models.Dtos;
using CryptoApp.Services.Models.Response;
using Microsoft.Extensions.Options;

namespace CryptoApp.Services.Implementations;

public class CryptoApiService: ICryptoApiService
{
    private readonly HttpClient _httpClient;
    private readonly CryptoApiOptions _options;    
    public CryptoApiService(HttpClient httpClient, IOptions<CryptoApiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "CryptoPortfolioApp/1.0");
        
        if (!string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            _httpClient.DefaultRequestHeaders.Add("x-cg-demo-api-key", _options.ApiKey);
        }
        
    }

    public async Task<IEnumerable<CryptoCurrency>> GetTopCurrenciesAsync(int count = 50, CancellationToken cancellationToken = default)
    {
        var url = $"{_options.BaseUrl}/coins/markets?vs_currency=usd&order=market_cap_desc&per_page={count}&page=1&sparkline=false";
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
                PriceUsd = dto.CurrentPrice ?? 0m,
                ChangePercent24Hr = dto.PriceChangePercentage24H ?? 0m,
                MarketCap = dto.MarketCap ?? 0m,
                Volume24Hr = dto.TotalVolume ?? 0m
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching top currencies: {ex.Message}");
            throw;
        }
    }
    public async Task<IEnumerable<CryptoCurrency>> SearchCurrenciesAsync(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(query))
            return [];
        
        var url = $"{_options.BaseUrl}/search?query={Uri.EscapeDataString(query)}";
        try
        {
            var searchResponse = await _httpClient.GetAsync(url, cancellationToken);
            searchResponse.EnsureSuccessStatusCode();
        
            var searcgContent = await searchResponse.Content.ReadAsStringAsync(cancellationToken);
            var searchResult = JsonSerializer.Deserialize<CoinGeckoSearchResponse>(searcgContent);
        
            if (searchResult?.Coins == null || !searchResult.Coins.Any())
                return [];
        
            var ids = string.Join(",", searchResult.Coins.Take(5).Select(c => c.Id));
        
            var marketsUrl = $"{_options.BaseUrl}/coins/markets?vs_currency=usd&ids={ids}";
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
                PriceUsd = dto.CurrentPrice ?? 0m,
                ChangePercent24Hr = dto.PriceChangePercentage24H ?? 0m,
                MarketCap = dto.MarketCap ?? 0m,
                Volume24Hr = dto.TotalVolume ?? 0m
            });
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error searching for currencies with query '{query}': {e.Message}");
            throw;
        }
    }

    public async Task<CryptoCurrency?> GetCurrencyDetailsAsync(string id, CancellationToken cancellationToken = default)
    {
        var url = $"{_options.BaseUrl}/coins/markets?vs_currency=usd&ids={Uri.EscapeDataString(id)}";
        try
        {
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
                PriceUsd = dto.CurrentPrice ?? 0m,
                ChangePercent24Hr = dto.PriceChangePercentage24H ?? 0m,
                MarketCap = dto.MarketCap ?? 0m,
                Volume24Hr = dto.TotalVolume ?? 0m
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching details for currency {id}: {e.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<MarketInfo>> GetMarketsForCurrencyAsync(string id, CancellationToken cancellationToken = default)
    {
        var url = $"{_options.BaseUrl}/coins/{Uri.EscapeDataString(id)}/tickers?include_exchange_logo=false&order=volume_desc";

        try
        {
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching markets for currency {id}: {ex.Message}");
            throw;
        }
    }
}
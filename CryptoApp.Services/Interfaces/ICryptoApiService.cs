using CryptoApp.Models.Entities;

namespace CryptoApp.Services.Interfaces;

public interface ICryptoApiService
{
    Task<IEnumerable<CryptoCurrency>> GetTopCurrenciesAsync(int count = 50, CancellationToken cancellationToken = default);
    Task<IEnumerable<CryptoCurrency>> SearchCurrenciesAsync(string query, CancellationToken cancellationToken = default);
    Task<CryptoCurrency?> GetCurrencyDetailsAsync(string id, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<MarketInfo>> GetMarketsForCurrencyAsync(string id, CancellationToken cancellationToken = default);
}
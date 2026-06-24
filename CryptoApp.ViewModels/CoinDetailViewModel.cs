using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryptoApp.Models.Entities;
using CryptoApp.Services.Interfaces;
using CryptoApp.ViewModels.Base;
using CryptoApp.ViewModels.Services;

namespace CryptoApp.ViewModels;

public partial class CoinDetailViewModel: ViewModelBase
{
    private readonly ICryptoApiService _apiService;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

    
    [ObservableProperty]
    private CryptoCurrency _coin = null!;
    [ObservableProperty]
    private ObservableCollection<MarketInfo> _markets = new();
    
    [ObservableProperty]
    private bool _isLoading;
    
    public CoinDetailViewModel(
        ICryptoApiService apiService, 
        IDialogService dialogService, 
        INavigationService navigationService)
    {
        _apiService = apiService;
        _dialogService = dialogService;
        _navigationService = navigationService;
    }
    
    public void Initialize(CryptoCurrency coin)
    {
        Coin = coin;
        LoadMarketsCommand.ExecuteAsync(null);
        LoadCoinDetailsCommand.ExecuteAsync(null);
    }
    
    [ObservableProperty]
    private string _lastUpdatedText = "Ще не оновлено";
    
    [RelayCommand]
    private async Task LoadMarketsAsync()
    {
        IsLoading = true;
        try
        {
            var data = await _apiService.GetMarketsForCurrencyAsync(Coin.Id);
            Markets = new ObservableCollection<MarketInfo>(data);
            LastUpdatedText = $"Оновлено: {DateTime.Now:HH:mm:ss}";
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            _dialogService.ShowError("Забагато запитів. Не вдалося завантажити біржі.", "Ліміт API");
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Помилка: {ex.Message}", "Помилка");
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    [RelayCommand]
    private async Task LoadCoinDetailsAsync()
    {
        try
        {
            var fullDetails = await _apiService.GetCurrencyDetailsAsync(Coin.Id);
            if (fullDetails != null)
            {
                Coin = fullDetails; 
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Не вдалося оновити деталі монети: {ex.Message}", "Помилка");
        }
    }
    
    [RelayCommand]
    private void GoBack()
    {
        _navigationService.GoBack();
    }
    
    [RelayCommand]
    private void OpenUrl(string? url)
    {
        if (!string.IsNullOrWhiteSpace(url))
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
    }
}
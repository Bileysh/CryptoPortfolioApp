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
    private readonly ViewModelBase _homeViewModel;
    
    [ObservableProperty]
    private CryptoCurrency _coin;
    [ObservableProperty]
    private ObservableCollection<MarketInfo> _markets = new();
    [ObservableProperty]
    private bool _isLoading;
    
    public CoinDetailViewModel(
        CryptoCurrency coin, 
        ICryptoApiService apiService, 
        IDialogService dialogService, 
        INavigationService navigationService,
        ViewModelBase homeViewModel)
    {
        Coin = coin; 
        _apiService = apiService;
        _dialogService = dialogService;
        _navigationService = navigationService;
        _homeViewModel = homeViewModel;

        LoadMarketsCommand.ExecuteAsync(null);
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
    private void GoBack()
    {
        _navigationService.NavigateTo(_homeViewModel);
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
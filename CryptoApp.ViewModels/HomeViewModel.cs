using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryptoApp.Models.Entities;
using CryptoApp.Services.Interfaces;
using CryptoApp.ViewModels.Base;

namespace CryptoApp.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    private readonly ICryptoApiService _apiService;
    private readonly IDialogService _dialogService;
    
    [ObservableProperty] 
    private ObservableCollection<CryptoCurrency> _currencies = new();
    
    [ObservableProperty]
    private bool _isLoading;
    
    [ObservableProperty]
    private string _searchText = string.Empty;
    
    public HomeViewModel(ICryptoApiService apiService, IDialogService dialogService)
    {
        _apiService = apiService;
        _dialogService = dialogService;
        LoadDataCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        if (IsLoading) return;
        
        IsLoading = true;
        try
        {
            var data = await _apiService.GetTopCurrenciesAsync();
            Currencies = new ObservableCollection<CryptoCurrency>(data);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            _dialogService.ShowError("Перевищено ліміт запитів до CoinGecko (максимум 10-30 на хвилину). Зачекайте хвилинку.", "Ліміт API");
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Сталася помилка:\n{ex.Message}", "Помилка");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            await LoadDataAsync();
            return;
        }
        
        IsLoading = true;
        try
        {
            var data = await _apiService.SearchCurrenciesAsync(SearchText);
            Currencies = new ObservableCollection<CryptoCurrency>(data);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            _dialogService.ShowError("Перевищено ліміт запитів до CoinGecko при пошуку. Зачекайте хвилину.","Ліміт API");
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Помилка при пошуку:\n{ex.Message}", "Помилка мережі");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
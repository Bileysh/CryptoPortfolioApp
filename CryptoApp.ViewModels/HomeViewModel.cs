using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryptoApp.Models.Entities;
using CryptoApp.Services.Interfaces;
using CryptoApp.ViewModels.Base;
using CryptoApp.ViewModels.Services;
using System.Threading;


namespace CryptoApp.ViewModels;

public partial class HomeViewModel : ViewModelBase, IDisposable
{
    private readonly ICryptoApiService _apiService;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;
    private readonly Func<CryptoCurrency, CoinDetailViewModel> _coinDetailFactory;
    
    private PeriodicTimer? _timer;
    private CancellationTokenSource? _timerCts;
    
    public HomeViewModel(ICryptoApiService apiService, IDialogService dialogService, INavigationService navigationService, Func<CryptoCurrency, CoinDetailViewModel> coinDetailFactory)
    {
        _apiService = apiService;
        _dialogService = dialogService;
        _navigationService = navigationService;
        _coinDetailFactory = coinDetailFactory;

        LoadDataCommand.ExecuteAsync(null);
        
        StartAutoUpdate();
    }
    private async void StartAutoUpdate()
    {
        _timerCts = new CancellationTokenSource();
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(60)); 

        try
        {
            while (await _timer.WaitForNextTickAsync(_timerCts.Token))
            {
                if (!IsLoading)
                {
                    await LoadDataCommand.ExecuteAsync(null);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    }
    
    public void Dispose()
    {
        _timerCts?.Cancel();
        _timerCts?.Dispose();
        _timer?.Dispose();
    }
    
    [ObservableProperty]
    private bool _isLoading;
    
    [ObservableProperty]
    private string _searchText = string.Empty;
    
    [ObservableProperty] 
    private ObservableCollection<CryptoCurrency> _currencies = new();
    
    [ObservableProperty] 
    private CryptoCurrency? _selectedCurrency;
    
    [ObservableProperty]
    private string _lastUpdatedText = "Ще не оновлено";
    
    [RelayCommand]
    private async Task LoadDataAsync()
    {
        if (IsLoading) return;
        
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            await SearchAsync();
            return;
        }
        
        IsLoading = true;
        try
        {
            var data = await _apiService.GetTopCurrenciesAsync();
            Currencies = new ObservableCollection<CryptoCurrency>(data);
            LastUpdatedText = $"Оновлено: {DateTime.Now:HH:mm:ss}";
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
            LastUpdatedText = $"Оновлено: {DateTime.Now:HH:mm:ss}";
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
    
    partial void OnSelectedCurrencyChanged(CryptoCurrency? value)
    {
        if (value != null)
        {
            var detailViewModel = _coinDetailFactory(value);

            _navigationService.NavigateTo(detailViewModel);

            SynchronizationContext.Current?.Post(_ =>
            {
                SelectedCurrency = null;
            }, null);
        }
    }
}
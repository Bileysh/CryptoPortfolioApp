using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryptoApp.ViewModels.Base;
using CryptoApp.ViewModels.Services;

namespace CryptoApp.ViewModels;

public partial class MainViewModel: ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IThemeService _themeService;
    
    [ObservableProperty]
    private bool _isDarkTheme;
    public ViewModelBase CurrentViewModel => _navigationService.CurrentView;
    
    public MainViewModel(INavigationService navigationService, HomeViewModel homeViewModel, IThemeService themeService)
    {
        _navigationService = navigationService;
        _themeService = themeService;
        _navigationService.StateChanged += OnStateChanged;
        _navigationService.NavigateTo(homeViewModel);
    }
    
    private void OnStateChanged()
    {
        OnPropertyChanged(nameof(CurrentViewModel));
    }
    
    
    
    [RelayCommand]
    private void ToggleTheme()
    {
        IsDarkTheme = !IsDarkTheme;
        _themeService.SetTheme(IsDarkTheme); 
    }
}
using CryptoApp.ViewModels.Base;
using CryptoApp.ViewModels.Services;

namespace CryptoApp.ViewModels;

public class MainViewModel: ViewModelBase
{
    private readonly INavigationService _navigationService;

    public ViewModelBase CurrentViewModel => _navigationService.CurrentView;
    
    public MainViewModel(INavigationService navigationService, HomeViewModel homeViewModel)
    {
        _navigationService = navigationService;
        _navigationService.StateChanged += OnStateChanged;
        _navigationService.NavigateTo(homeViewModel);
    }
    
    private void OnStateChanged()
    {
        OnPropertyChanged(nameof(CurrentViewModel));
    }
}
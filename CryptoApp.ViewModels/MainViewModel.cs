using CryptoApp.ViewModels.Base;

namespace CryptoApp.ViewModels;

public class MainViewModel: ViewModelBase
{
    private ViewModelBase _currentViewModel;

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }
    
    public MainViewModel(HomeViewModel homeViewModel)
    {
        _currentViewModel = homeViewModel;
    }
}
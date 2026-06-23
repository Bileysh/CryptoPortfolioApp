using CryptoApp.ViewModels.Base;
using CryptoApp.ViewModels.Services;

namespace CryptoApp.WPF.Services;

public class NavigationService: INavigationService
{
    private ViewModelBase _currentView = null!;
    
    public ViewModelBase CurrentView
    {
        get => _currentView;
        private set
        {
            _currentView = value;
            StateChanged?.Invoke();
        }
    }
    
    public event Action? StateChanged;
    
    public void NavigateTo(ViewModelBase viewModel)
    {
        CurrentView = viewModel;
    }
}
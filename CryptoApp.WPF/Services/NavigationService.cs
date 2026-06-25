using CryptoApp.ViewModels.Base;
using CryptoApp.ViewModels.Services;

namespace CryptoApp.WPF.Services;

public class NavigationService: INavigationService
{
    private ViewModelBase _currentView = null!;
    
    private readonly Stack<ViewModelBase> _history = new();

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
        _history.Push(_currentView);
        CurrentView = viewModel;
    }
    
    public void GoBack()
    {
        if (_history.Count > 0)
        {
            CurrentView = _history.Pop();
        }
    }
}
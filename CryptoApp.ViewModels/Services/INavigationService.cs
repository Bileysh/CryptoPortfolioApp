using CryptoApp.ViewModels.Base;

namespace CryptoApp.ViewModels.Services;

public interface INavigationService
{
    ViewModelBase CurrentView { get; }
    event Action? StateChanged;
    void NavigateTo(ViewModelBase viewModel);
}
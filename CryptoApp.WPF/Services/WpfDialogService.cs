using System.Windows;
using CryptoApp.Services.Interfaces;

namespace CryptoApp.WPF.Services;

public class WpfDialogService : IDialogService
{
    public void ShowMessage(string message, string title)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public void ShowError(string message, string title)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}
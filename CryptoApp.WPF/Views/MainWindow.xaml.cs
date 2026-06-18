using System.Windows;
using CryptoApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoApp.WPF.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
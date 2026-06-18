using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using CryptoApp.ViewModels;
using CryptoApp.WPF.Views;

namespace CryptoApp.WPF
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<HomeViewModel>();
        }
    }
}
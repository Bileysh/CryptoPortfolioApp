using System;
using System.Windows;
using CryptoApp.Services.Implementations;
using CryptoApp.Services.Interfaces;
using CryptoApp.Services.Models;
using Microsoft.Extensions.DependencyInjection;
using CryptoApp.ViewModels;
using CryptoApp.WPF.Services;
using CryptoApp.WPF.Views;
using Microsoft.Extensions.Configuration;

namespace CryptoApp.WPF
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<App>() 
                .Build();
            
            var services = new ServiceCollection();
            
            services.AddSingleton<IConfiguration>(configuration);
            
            ConfigureServices(services, configuration);
            ServiceProvider = services.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            
            services.Configure<CryptoApiOptions>(options =>
            {
                options.ApiKey = configuration["CoinGeckoApiKey"] ?? string.Empty;
            });
            
            services.AddSingleton<MainWindow>();
            
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<HomeViewModel>();
            
            services.AddHttpClient<ICryptoApiService, CryptoApiService>();
            
            services.AddSingleton<IDialogService, WpfDialogService>();
        }
    }
}
using System;
using System.Windows;
using CryptoApp.Models.Entities;
using CryptoApp.Services.Implementations;
using CryptoApp.Services.Interfaces;
using CryptoApp.Services.Models;
using Microsoft.Extensions.DependencyInjection;
using CryptoApp.ViewModels;
using CryptoApp.ViewModels.Services;
using CryptoApp.WPF.Services;
using CryptoApp.WPF.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


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
            services.AddTransient<HomeViewModel>();
            services.AddTransient<CoinDetailViewModel>();
            
            services.AddHttpClient<ICryptoApiService, CryptoApiService>();
            
            services.AddSingleton<IDialogService, WpfDialogService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IThemeService, ThemeService>();
            
            services.AddLogging(builder =>
            {
                builder.AddDebug();
            });
            
            services.AddSingleton<Func<CryptoCurrency, CoinDetailViewModel>>(sp => 
                coin => 
                {
                    var vm = sp.GetRequiredService<CoinDetailViewModel>();
                    vm.Initialize(coin);
                    return vm;
                });
        }
    }
}
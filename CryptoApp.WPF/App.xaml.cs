using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using CryptoApp.ViewModels;
using CryptoApp.WPF.Views;

namespace CryptoApp.WPF
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();
            base.OnStartup(e);

        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MainViewModel>();
            services.AddTransient<HomeViewModel>();
        }
    }
}
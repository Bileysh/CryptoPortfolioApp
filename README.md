#  Crypto Portfolio App

A WPF desktop application for tracking cryptocurrency prices in real time, built with C# / .NET 8 and Clean Architecture.

##  Implemented Features

### Core (Required)
- Multi-page application with navigation (Home → Coin Detail → Back)
- Home page with Top 50 cryptocurrencies by market cap
- Search by coin name or symbol (via CoinGecko API)
- Coin detail page: price, 24h change, market cap, volume, ATH, circulating supply
- Markets table — list of exchanges where the coin is traded, with direct links
- MVVM pattern with CommunityToolkit.Mvvm (Source Generators)

### Bonus
- Light / Dark theme toggle (☽ / ☼ button in the header)
- Auto-refresh every 60 seconds on the home page
- Navigation stack with GoBack() support
- Color-coded 24h change (green / red)
- Loading indicators on all pages


##  Architecture

The project follows **Clean Architecture** with four separate projects in the Solution:

```
CryptoPortfolioApp/
├── CryptoApp.Models        # Domain entities (CryptoCurrency, MarketInfo)
├── CryptoApp.Services      # API service, interfaces, DTOs
├── CryptoApp.ViewModels    # ViewModels, navigation & theme interfaces
└── CryptoApp.WPF           # Views, styles, converters, DI bootstrap
```

Dependencies flow inward only — WPF depends on ViewModels, ViewModels depend on Services, Services depend on Models.

---

## Tech Stack

| Component | Technology |
|---|---|
| Platform | .NET 8 / WPF |
| Language | C# 12 |
| MVVM | CommunityToolkit.Mvvm |
| Dependency Injection | Microsoft.Extensions.DependencyInjection |
| HTTP | HttpClient via IHttpClientFactory |
| JSON | System.Text.Json |
| Crypto API | CoinGecko (Free tier) |
| Config / Secrets | Microsoft.Extensions.Configuration + User Secrets |
| Logging | Microsoft.Extensions.Logging.Debug |


## How to Run

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 (with .NET desktop workload) or Rider

### Steps

1. **Clone the repository**
   ```bash
   git clone https://github.com/Bileysh/CryptoPortfolioApp.git
   cd CryptoPortfolioApp
   ```

2. **Set up the API key** (optional — the app works without it on the free tier, but a key increases the rate limit)

   Get a free Demo API key at [coingecko.com/en/api](https://www.coingecko.com/en/api/pricing), then add it via User Secrets:
   ```bash
   cd CryptoApp.WPF
   dotnet user-secrets set "CoinGeckoApiKey" "YOUR_KEY_HERE"
   ```
   If you skip this step the app still works — just limited to ~10–30 requests/minute.

3. **Open and run**

   Open `CryptoPortfolioApp.sln` in Visual Studio, set `CryptoApp.WPF` as the startup project, and press **F5**.



## Key Design Decisions

**DTO / Domain separation** — API responses are deserialized into `CoinGecko*Dto` classes and then mapped to clean domain models (`CryptoCurrency`, `MarketInfo`). Views never touch DTOs.

**Navigation** — `INavigationService` with a `Stack<ViewModelBase>` history. `MainWindow` has a single `ContentControl` bound to `CurrentViewModel`; WPF `DataTemplate` entries in `DataTemplates.xaml` map each ViewModel to its View automatically.

**Factory for CoinDetailViewModel** — `HomeViewModel` receives a `Func<CryptoCurrency, CoinDetailViewModel>` from DI instead of creating the ViewModel manually, keeping the DI graph intact.

**Theming** — `LightTheme.xaml` / `DarkTheme.xaml` define named `SolidColorBrush` resources (`AppBackground`, `TextPrimary`, etc.). All styles reference them via `{DynamicResource}`, so swapping a single `MergedDictionary` entry changes the entire UI instantly.

**API key security** — stored in .NET User Secrets, never committed to source control.


## Known Limitations

- CoinGecko Free tier: ~10–30 requests/minute. The app shows a clear message if the limit is hit.
- The Detail page makes two API calls on open (markets + coin details) — on a slow connection or with rate limiting both may not complete simultaneously.

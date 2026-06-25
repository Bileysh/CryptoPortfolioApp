using System.Windows;
using CryptoApp.ViewModels.Services;

namespace CryptoApp.WPF.Services;

public class ThemeService: IThemeService
{
    public void SetTheme(bool isDark)
    {
        string themePath = isDark 
            ? "pack://application:,,,/Resources/DarkTheme.xaml" 
            : "pack://application:,,,/Resources/LightTheme.xaml";
        
        var newThemeDict = new ResourceDictionary { Source = new Uri(themePath, UriKind.Absolute) };
        
        var dictionaries = Application.Current.Resources.MergedDictionaries;
        
        var oldTheme = dictionaries.FirstOrDefault(d => d.Source != null && 
                                                        (d.Source.OriginalString.Contains("LightTheme") || d.Source.OriginalString.Contains("DarkTheme")));
        if (oldTheme != null)
        {
            dictionaries.Remove(oldTheme);
        }
        dictionaries.Insert(0, newThemeDict);
    }
}
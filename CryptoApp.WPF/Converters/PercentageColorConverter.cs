using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CryptoApp.WPF.Converters;

public class PercentageColorConverter: IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if(value is decimal percentage)
        {
            if (percentage > 0)
                return Brushes.ForestGreen;
            if (percentage < 0)
                return Brushes.Crimson;
        }
        return Brushes.Black;

    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
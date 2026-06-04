using System.Globalization;
using LifeChart.Application.Settings;

namespace LifeChart.Converters;

public class IsNextcloudConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is CloudProvider.Nextcloud;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

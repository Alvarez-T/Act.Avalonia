using Avalonia.Data.Converters;
using System.Globalization;

namespace Dotfy.Avalonia.UI.Converters;

public class BadgeContentOverflowConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        string overflowMark = parameter is string s ? s : "+";
        if (double.TryParse(values[0]?.ToString(), out var b) && values[1] is int i and > 0)
        {
            if (b > i)
            {
                return i + overflowMark;
            }
        }
        return values[0];
    }
}
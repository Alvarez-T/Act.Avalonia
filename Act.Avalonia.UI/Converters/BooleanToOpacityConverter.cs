﻿using System.Globalization;
using Avalonia.Data.Converters;

namespace Act.Avalonia.UI.Converters;

public class BooleanToOpacityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
        {
            return b ? 1.0 : 0.0;
        }
        return 1;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
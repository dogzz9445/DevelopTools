﻿using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace DDT.Core.WidgetSystems.Converters;

/// <summary>
/// Converter that checks the value for null and if it is null returns collapsed; else visible
/// Implements the <see cref="System.Windows.Data.IValueConverter" />
/// </summary>
/// <seealso cref="System.Windows.Data.IValueConverter" />
public class NullToVisibilityConverter : IValueConverter
{
    #region Public Methods

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value == null ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    #endregion Public Methods
}

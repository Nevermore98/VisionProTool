﻿using System.Globalization;
using System.Windows.Data;

namespace WPF_VisionPro_Demo.Converters
{
    public class ObjectToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                value = intValue;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
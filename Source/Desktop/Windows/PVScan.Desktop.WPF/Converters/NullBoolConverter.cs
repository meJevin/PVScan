using System;
using System.Globalization;
using System.Windows.Data;

namespace PVScan.Desktop.WPF.Converters
{
    public class NullBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object inverse, CultureInfo culture)
        {
            return (value == null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

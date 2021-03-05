using System;
using System.Globalization;
using Xamarin.Forms;

namespace PVScan.Mobile.Converters
{
    public class NullBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object inverse, CultureInfo culture)
        {
            return (value == null) && (!(bool)inverse);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

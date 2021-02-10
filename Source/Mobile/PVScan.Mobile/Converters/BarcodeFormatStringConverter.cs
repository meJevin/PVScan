using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;
using ZXing;

namespace PVScan.Mobile.Converters
{
    public class BarcodeFormatStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            BarcodeFormat format = (BarcodeFormat)value;

            return Enum.GetName(typeof(BarcodeFormat), format).Replace('_', ' ');
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

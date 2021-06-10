using PVScan.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using ZXing;

namespace PVScan.Desktop.WPF.Converters
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

            string result = Enum.GetName(typeof(BarcodeFormat), format).Replace('_', ' ');

            // Title Case
            if (parameter != null && (string)parameter == "TitleCase")
            {
                return result.ToLower().ToTitleCase();
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

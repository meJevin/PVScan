using MapboxNetCore;
using PVScan.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using ZXing;

namespace PVScan.Desktop.WPF.Converters
{
    public class GeoLocationStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            GeoLocation location = (GeoLocation)value;

            return $"{location.Longitude.ToString("0.00000")}, {location.Latitude.ToString("0.00000")}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

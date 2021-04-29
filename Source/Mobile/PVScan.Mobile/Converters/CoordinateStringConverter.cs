using System;
using System.Globalization;
using PVScan.Mobile.Models;
using Xamarin.Forms;

namespace PVScan.Mobile.Converters
{
    public class CoordinateStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Coordinate coord = value as Coordinate;

            if (coord == null)
            {
                return null;
            }

            return $"{coord.Latitude}, {coord.Longitude}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

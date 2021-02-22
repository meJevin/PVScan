using PVScan.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace PVScan.Mobile.Converters
{
    public class CoordinatePositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Coordinate coord)
            {
                if (coord.Latitude.HasValue && coord.Longitude.HasValue)
                {
                    return new Position(coord.Latitude.Value, coord.Longitude.Value);
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

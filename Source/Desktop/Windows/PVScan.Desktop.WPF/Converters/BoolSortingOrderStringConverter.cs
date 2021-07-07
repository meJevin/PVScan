using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace PVScan.Desktop.WPF.Converters
{
    public class BoolSortingOrderStringConverter : IValueConverter
    {
        // True for descending false for ascending
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }

            var desc = (bool)value;
            return desc ? "descendingly" : "ascendingly";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

using System;
using System.Globalization;
using PVScan.Mobile.ViewModels;
using Xamarin.Forms;

namespace PVScan.Mobile.Converters
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

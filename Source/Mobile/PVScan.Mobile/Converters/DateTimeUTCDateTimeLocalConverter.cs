using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace PVScan.Mobile.Converters
{
    public class DateTimeUTCDateTimeLocalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime timeUtc = DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc);

            return timeUtc.ToLocalTime();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

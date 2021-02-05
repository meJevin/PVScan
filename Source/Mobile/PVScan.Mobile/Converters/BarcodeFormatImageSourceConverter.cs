using PVScan.Mobile.Styles;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;
using ZXing;

namespace PVScan.Mobile.Converters
{
    public class BarcodeFormatImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BarcodeFormat format = (BarcodeFormat)value;

            var result = new FontImageSource()
            {
                FontFamily = "FontAwesome",
                Glyph = IconFont.Barcode,
            };
            
            try
            {
                result.SetOnAppTheme(FontImageSource.ColorProperty,
                    Application.Current.Resources["TextPrimaryColor_Light"],
                    Application.Current.Resources["TextPrimaryColor_Dark"]);
            }
            catch
            {

            }

            if (format.HasFlag(BarcodeFormat.QR_CODE))
            {
                result.Glyph = IconFont.Qrcode;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

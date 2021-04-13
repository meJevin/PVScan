using System;
using System.Globalization;
using System.IO;
using System.Text;
using PVScan.Mobile.Models;
using Xamarin.Forms;
using Xamarin.Forms.Svg;
using ZXing;
using ZXing.Common;

namespace PVScan.Mobile.Converters
{
    public class BarcodeImageConverter : IValueConverter
    {
        BarcodeWriterSvg writer;
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                Barcode b = value as Barcode;

                if (b == null)
                {
                    return null;
                }

                writer = new BarcodeWriterSvg()
                {
                    Format = b.Format,
                };

                SetBarcodeWriterSizeForBarcode(writer, b);

                var svg = writer.Write(b.Text);

                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(svg.Content));

                var result = SvgImageSource.FromSvgStream(() => ms, writer.Options.Width, writer.Options.Height, Color.Black);

                return result;
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        private void SetBarcodeWriterSizeForBarcode(BarcodeWriterSvg writer, Barcode b)
        {
            if (b.Format == BarcodeFormat.AZTEC ||
                b.Format == BarcodeFormat.DATA_MATRIX ||
                b.Format == BarcodeFormat.MAXICODE ||
                b.Format == BarcodeFormat.QR_CODE)
            {
                writer.Options = new EncodingOptions()
                {
                    Width = 600,
                    Height = 600,
                };
            }
            else if (b.Format == BarcodeFormat.CODABAR)
            {
                writer.Options = new EncodingOptions()
                {
                    Width = 1320,
                    Height = 600,
                };
            }
            else if (b.Format == BarcodeFormat.CODE_128 ||
                b.Format == BarcodeFormat.CODE_39 ||
                b.Format == BarcodeFormat.CODE_93 ||
                b.Format == BarcodeFormat.EAN_13 ||
                b.Format == BarcodeFormat.EAN_8)
            {
                writer.Options = new EncodingOptions()
                {
                    Width = 1800,
                    Height = 600,
                };
            }
            else if (b.Format == BarcodeFormat.IMB)
            {
                writer.Options = new EncodingOptions()
                {
                    Width = 1800,
                    Height = 600,
                };
            }
            else if (b.Format == BarcodeFormat.ITF ||
                b.Format == BarcodeFormat.MSI ||
                b.Format == BarcodeFormat.PLESSEY ||
                b.Format == BarcodeFormat.RSS_14)
            {
                writer.Options = new EncodingOptions()
                {
                    Width = 2400,
                    Height = 600,
                };
            }
            else if (b.Format == BarcodeFormat.PDF_417)
            {
                writer.Options = new EncodingOptions()
                {
                    Width = 1600,
                    Height = 600,
                };
            }
            else if (b.Format == BarcodeFormat.UPC_A ||
                b.Format == BarcodeFormat.UPC_E ||
                b.Format == BarcodeFormat.UPC_EAN_EXTENSION)
            {
                writer.Options = new EncodingOptions()
                {
                    Width = 1020,
                    Height = 600,
                };
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

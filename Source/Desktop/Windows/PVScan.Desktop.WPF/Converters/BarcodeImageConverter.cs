using System;
using System.Globalization;
using System.IO;
using System.Text;
using PVScan.Core.Models;
using System.Windows.Data;
using ZXing;
using ZXing.Common;
using static ZXing.Rendering.SvgRenderer;
using System.Windows.Controls;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using System.Windows.Media.Imaging;

namespace PVScan.Desktop.WPF.Converters
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

                var localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var tempSVGFileDir = Path.Combine(localAppDataFolder, "PVScan", "Temp");
                var tempSVGFilePath = Path.Combine(tempSVGFileDir, $"BarcodeSVG_temp.svg");

                // Todo: move this somewhere else..
                if (!Directory.Exists(tempSVGFileDir))
                {
                    Directory.CreateDirectory(tempSVGFileDir);
                }

                if (!File.Exists(tempSVGFilePath))
                {
                    File.Create(tempSVGFilePath);
                }

                // Todo: Bug with SVG renderer putting invalid rgba value
                svg.Content = svg.Content.Replace("rgba(1)", "rgba(255,255,255,0)");
                File.WriteAllText(tempSVGFilePath, svg.Content);

                var cnv = new ImageSvgConverter(new WpfDrawingSettings()
                {
                    IncludeRuntime = true,
                    TextAsGeometry = true,
                });

                var result = cnv.Convert(tempSVGFilePath);

                return LoadBitmapImage(tempSVGFilePath.Replace(".svg", ".png"));
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

        public static BitmapImage LoadBitmapImage(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

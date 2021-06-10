using Emgu.CV;
using Emgu.CV.Structure;
using PVScan.Desktop.WPF.Models;
using PVScan.Desktop.WPF.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace PVScan.Desktop.WPF.Services
{
    public class BarcodeReaderImage : BarcodeReader<Image<Bgr, byte>>, IBarcodeReaderImage
    {
        private static readonly Func<Image<Bgr, byte>, LuminanceSource> defaultCreateLuminanceSource =
           (image) => new ImageLuminanceSource(image);

        public BarcodeReaderImage()
           : base(null, defaultCreateLuminanceSource, null)
        {
        }
    }
}

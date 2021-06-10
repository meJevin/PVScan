using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace PVScan.Desktop.WPF.Services.Interfaces
{
    /// <summary>
    /// The interface for a barcode reader which accepts an Image instance from EmguCV
    /// </summary>
    public interface IBarcodeReaderImage : IBarcodeReader<Image<Bgr, byte>>
    {
    }
}

using Emgu.CV;
using Emgu.CV.Structure;

using PVScan.Core.Models;
using PVScan.Core.Services.Interfaces;
using PVScan.Desktop.WPF.Services.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace PVScan.Desktop.WPF.ViewModels
{
    public class ScanPageViewModel : BaseViewModel
    {
        readonly IBarcodeReaderImage BarcodeReader;

        VideoCapture capture;
        Mat frame;

        public ScanPageViewModel(IBarcodeReaderImage barcodeReader)
        {
            BarcodeReader = barcodeReader;

            frame = new Mat();

            capture = new VideoCapture(0);
            capture.Start();

            capture.ImageGrabbed += (s, e) =>
            {
                if (capture.Retrieve(frame, 0))
                {
                    Dispatcher.CurrentDispatcher.Invoke(() =>
                    {
                        VideoCapture_ImageGrabbed();
                    });
                }
            };
        }

        private void VideoCapture_ImageGrabbed()
        {
            CurrentFrameImage = frame.ToBitmap().ToBitmapImageSource();

            var tempResult = BarcodeReader.Decode(frame.ToImage<Bgr, byte>());

            if (tempResult != null)
            {
                LastScannedBarcode = tempResult;
            }
        }

        public ZXing.Result LastScannedBarcode { get; set; }
        public ImageSource CurrentFrameImage { get; set; }
    }
}

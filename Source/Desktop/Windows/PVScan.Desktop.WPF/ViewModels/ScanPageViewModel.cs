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
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Windows.Devices.Geolocation;
using ZXing;

namespace PVScan.Desktop.WPF.ViewModels
{
    public class ScanPageViewModel : BaseViewModel
    {
        readonly IBarcodeReaderImage BarcodeReader;
        readonly IBarcodesRepository BarcodesRepository;

        public ScanPageViewModel(IBarcodeReaderImage barcodeReader, IBarcodesRepository repository)
        {
            BarcodeReader = barcodeReader;
            BarcodesRepository = repository;

            frame = new Mat();

            capture = new VideoCapture(0);
            capture.Start();

            capture.ImageGrabbed += (s, e) =>
            {
                if (capture.Retrieve(frame, 0))
                {
                    VideoCapture_ImageGrabbed();
                }
            };

            ClearCommand = new Command(() => 
            {
                LastScanResult = null;

                Cleared?.Invoke(this, new EventArgs());
            });

            SaveCommand = new Command(async () => 
            {
                Barcode barcodeToSave = new Barcode()
                {
                    Favorite = false,
                    Format = LastScanResult.BarcodeFormat,
                    GUID = Guid.NewGuid().ToString(),
                    ScanTime = DateTime.UtcNow,
                    Text = LastScanResult.Text,
                    ScanLocation = null,
                };

                // TOdo: get this into a service!
                var geolocationStatus = await Geolocator.RequestAccessAsync();

                if (geolocationStatus == GeolocationAccessStatus.Allowed)
                {
                    Geolocator geolocator = new Geolocator()
                    {
                        DesiredAccuracy = PositionAccuracy.High,
                    };
                    var location = await geolocator.GetGeopositionAsync();

                    barcodeToSave.ScanLocation = new Coordinate()
                    {
                        Latitude = location.Coordinate.Latitude,
                        Longitude = location.Coordinate.Longitude,
                    };
                }

                await BarcodesRepository.Save(barcodeToSave);

                ClearCommand.Execute(null);
            });

            FakeBarcodeCommand = new Command(() =>
            {
                LastScanResult = new Result("Test", null, null, BarcodeFormat.QR_CODE);

                GotBarcode?.Invoke(this, new EventArgs());
            });
        }

        private void VideoCapture_ImageGrabbed()
        {
            CurrentFrameImage = frame.ToBitmap().ToBitmapImageSource();

            var tempResult = BarcodeReader.Decode(frame.ToImage<Bgr, byte>());

            if (tempResult != null)
            {
                LastScanResult = tempResult;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    GotBarcode?.Invoke(this, new EventArgs());
                });
            }
        }

        public ICommand ClearCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand FakeBarcodeCommand { get; set; }

        public Result LastScanResult { get; set; }
        public ImageSource CurrentFrameImage { get; set; }

        public event EventHandler GotBarcode;
        public event EventHandler Saved;
        public event EventHandler Cleared;


        VideoCapture capture;
        Mat frame;
    }
}

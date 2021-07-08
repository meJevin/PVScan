using Emgu.CV;
using Emgu.CV.Structure;

using PVScan.Core.Models;
using PVScan.Core.Models.API;
using PVScan.Core.Services.Interfaces;
using PVScan.Desktop.WPF.Services.Interfaces;
using PVScan.Desktop.WPF.ViewModels.Messages.Scanning;
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
        readonly IPVScanAPI PVScanAPI;
        readonly IAPIBarcodeHub BarcodeHub;

        public ScanPageViewModel(
            IBarcodeReaderImage barcodeReader,
            IBarcodesRepository repository,
            IPVScanAPI pVScanAPI,
            IAPIBarcodeHub barcodeHub)
        {
            BarcodeReader = barcodeReader;
            BarcodesRepository = repository;
            PVScanAPI = pVScanAPI;
            BarcodeHub = barcodeHub;

            frame = new Mat();

            capture = new VideoCapture(0);
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
                Barcode b = new Barcode()
                {
                    Favorite = false,
                    Format = LastScanResult.BarcodeFormat,
                    GUID = Guid.NewGuid().ToString(),
                    ScanTime = DateTime.UtcNow,
                    Text = LastScanResult.Text,
                    ScanLocation = null,
                };

                // TOdo: get this into a service!
                var geolocationStatus = await Geolocator.RequestAccessAsync().AsTask();

                if (geolocationStatus == GeolocationAccessStatus.Allowed)
                {
                    Geolocator geolocator = new Geolocator()
                    {
                        DesiredAccuracy = PositionAccuracy.High,
                    };
                    var location = await geolocator.GetGeopositionAsync().AsTask();

                    b.ScanLocation = new Coordinate()
                    {
                        Latitude = location.Coordinate.Latitude,
                        Longitude = location.Coordinate.Longitude,
                    };
                }

                await BarcodesRepository.Save(b);

                MessagingCenter.Send(this, nameof(BarcodeScannedMessage), new BarcodeScannedMessage()
                {
                    ScannedBarcode = b,
                });

                ClearCommand.Execute(null);

                // Todo: move this somewhere else. Possibly in the IBarcodesRepository?
                var req = new ScannedBarcodeRequest()
                {
                    Format = b.Format,
                    Latitude = b.ScanLocation?.Latitude,
                    Longitude = b.ScanLocation?.Longitude,
                    ScanTime = b.ScanTime,
                    Text = b.Text,
                    Favorite = b.Favorite,
                    GUID = b.GUID,
                    Hash = b.Hash,
                    LastTimeUpdated = b.LastUpdateTime,
                };

                await PVScanAPI.ScannedBarcode(req);
                await BarcodeHub.Scanned(req);
            });

            FakeBarcodeCommand = new Command(() =>
            {
                for (int i = 0; i < 200; ++i)
                {
                    GenerateRandomBarcode();
                }
                LastScanResult = new Result("Test", null, null, BarcodeFormat.QR_CODE);

                GotBarcode?.Invoke(this, new EventArgs());
            });
        }

        public void StartCapturing()
        {
            capture.Start();
        }

        public void StopCapturing()
        {
            capture.Stop();
        }

        private void GenerateRandomBarcode()
        {
            Array values = Enum.GetValues(typeof(BarcodeFormat)).OfType<BarcodeFormat>().Where(f => { return (int)f <= 2048; }).ToArray();
            Random random = new Random();
            BarcodeFormat randomType = (BarcodeFormat)values.GetValue(random.Next(values.Length));

            DateTime date = DateTime.UtcNow;
            date = date.Subtract(TimeSpan.FromSeconds(random.NextDouble() * 157680000));

            Coordinate randomCoord = new Coordinate()
            {
                Latitude = (random.NextDouble() * 180) - 90,
                Longitude = (random.NextDouble() * 180) - 90,
            };

            var b = new Barcode()
            {
                Format = randomType,
                ScanLocation = random.NextDouble() > 0.25 ? randomCoord : null,
                ScanTime = date,
                Text = Guid.NewGuid().ToString(),
            };

            BarcodesRepository.Save(b);
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

using MvvmHelpers;
using PVScan.Mobile.DAL;
using PVScan.Mobile.Models;
using PVScan.Mobile.ViewModels.Messages.Scanning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing;

namespace PVScan.Mobile.ViewModels
{
    public class ScanPageViewModel : BaseViewModel
    {
        readonly PVScanMobileDbContext _context;

        public ScanPageViewModel()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "PVScan.db3");

            _context = new PVScanMobileDbContext(dbPath);

            ScanCommand = new Command(async (object scanResult) =>
            {
                LastResult = (scanResult as Result);

                LastBarcodeText = LastResult.Text;
                LastBarcodeType = Enum.GetName(typeof(BarcodeFormat), LastResult.BarcodeFormat).Replace('_', ' ');

                CanClear = true;
                CanSave = true;

                GotBarcode?.Invoke(this, new EventArgs());

                OnPropertyChanged(nameof(LastBarcodeText));
                OnPropertyChanged(nameof(LastBarcodeType));
                OnPropertyChanged(nameof(CanClear));
                OnPropertyChanged(nameof(CanSave));
            });

            ClearCommand = new Command(async () =>
            {
                LastResult = null;

                LastBarcodeText = null;
                LastBarcodeType = null;

                CanSave = false;
                CanClear = false;

                Cleared?.Invoke(this, new EventArgs());
                
                OnPropertyChanged(nameof(LastBarcodeText));
                OnPropertyChanged(nameof(LastBarcodeType));
                OnPropertyChanged(nameof(CanClear));
                OnPropertyChanged(nameof(CanSave));
            });

            SaveCommand = new Command(async () =>
            {
                CanClear = false;
                CanSave = false;

                OnPropertyChanged(nameof(CanClear));
                OnPropertyChanged(nameof(CanSave));

                // Save to DB and clear
                var location = await Geolocation.GetLocationAsync(new GeolocationRequest()
                {
                    DesiredAccuracy = GeolocationAccuracy.Best,
                    Timeout = TimeSpan.FromSeconds(5),
                });

                Console.WriteLine(location.ToString());

                // Todo: if user is logged in send this to server and set ServerSynced to true
                Barcode b = new Barcode()
                {
                    Format = LastResult.BarcodeFormat,
                    Text = LastResult.Text,
                    ScanLocation = new Coordinate()
                    {
                        Latitude = location.Latitude,
                        Longitude = location.Longitude,
                    },
                    ServerSynced = false,
                    ScanTime = DateTime.UtcNow,
                };

                await _context.Barcodes.AddAsync(b);
                await _context.SaveChangesAsync();

                Saved?.Invoke(this, new EventArgs());

                MessagingCenter.Send(this, nameof(BarcodeScannedMessage), new BarcodeScannedMessage()
                {
                    ScannedBarcode = b,
                });

                ClearCommand.Execute(null);
            });

            IsCameraAllowed = Permissions.CheckStatusAsync<Permissions.Camera>()
                .GetAwaiter().GetResult() == PermissionStatus.Granted;

            MessagingCenter.Subscribe(this, nameof(CameraAllowedMessage),
                async (ApplicationSettingsPageViewModel v, CameraAllowedMessage args) =>
                {
                    IsCameraAllowed = true;
                    OnPropertyChanged(nameof(IsCameraAllowed));
                });
        }

        private Result LastResult;

        public ICommand ScanCommand { get; }

        public ICommand ClearCommand { get; }
        
        public ICommand SaveCommand { get; }

        public bool CanClear { get; set; }

        public bool CanSave { get; set; }

        public bool IsCameraAllowed { get; set; }

        public string LastBarcodeText { get; set; }
        public string LastBarcodeType { get; set; }

        public event EventHandler GotBarcode;
        public event EventHandler Cleared;
        public event EventHandler Saved;
    }
}

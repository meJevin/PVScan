using MvvmHelpers;
using PVScan.Mobile.DAL;
using PVScan.Mobile.Models;
using PVScan.Mobile.Services.Interfaces;
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
        IBarcodesRepository BarcodesRepository;

        public ScanPageViewModel()
        {
            BarcodesRepository = Resolver.Resolve<IBarcodesRepository>();

            ScanCommand = new Command(async (object scanResult) =>
            {
                LastResult = (scanResult as Result);

                LastBarcodeText = LastResult.Text;
                LastBarcodeType = Enum.GetName(typeof(BarcodeFormat), LastResult.BarcodeFormat).Replace('_', ' ');

                CanClear = true;
                CanSave = true;

                GotBarcode?.Invoke(this, new EventArgs());
            });

            ClearCommand = new Command(async () =>
            {
                LastResult = null;

                LastBarcodeText = null;
                LastBarcodeType = null;

                CanSave = false;
                CanClear = false;

                Cleared?.Invoke(this, new EventArgs());
            });

            SaveCommand = new Command(async () =>
            {
                CanClear = false;
                CanSave = false;

                Location location = null;
                try
                {
                    location = await Geolocation.GetLocationAsync(new GeolocationRequest()
                    {
                        DesiredAccuracy = GeolocationAccuracy.Best,
                        Timeout = TimeSpan.FromSeconds(5),
                    });
                }
                catch (Exception e)
                {
                    // Inform user that location could not be fetched
                }

                // Todo: if user is logged in send this to server and set ServerSynced to true
                Barcode b = new Barcode()
                {
                    Format = LastResult.BarcodeFormat,
                    Text = LastResult.Text,
                    ServerSynced = false,
                    ScanLocation = new Coordinate()
                    {
                        Latitude = location?.Latitude,
                        Longitude = location?.Longitude,
                    },
                    ScanTime = DateTime.UtcNow,
                };

                b = await BarcodesRepository.Save(b);

                Saved?.Invoke(this, new EventArgs());

                MessagingCenter.Send(this, nameof(BarcodeScannedMessage), new BarcodeScannedMessage()
                {
                    ScannedBarcode = b,
                });

                ClearCommand.Execute(null);
            });

            AllowCameraCommand = new Command(async () =>
            {
                var result = await Permissions.RequestAsync<Permissions.Camera>();

                if (result == PermissionStatus.Granted)
                {
                    IsCameraAllowed = true;

                    CameraAllowed?.Invoke(this, new EventArgs());
                }
            });

            IsCameraAllowed = Permissions.CheckStatusAsync<Permissions.Camera>()
                .GetAwaiter().GetResult() == PermissionStatus.Granted;

            MessagingCenter.Subscribe(this, nameof(CameraAllowedMessage),
                async (ApplicationSettingsPageViewModel v, CameraAllowedMessage args) =>
                {
                    IsCameraAllowed = true;
                });
        }

        private Result LastResult;

        public ICommand ScanCommand { get; }

        public ICommand ClearCommand { get; }
        
        public ICommand SaveCommand { get; }

        public ICommand AllowCameraCommand { get; }

        public bool CanClear { get; set; }

        public bool CanSave { get; set; }

        public bool IsCameraAllowed { get; set; }

        public string LastBarcodeText { get; set; }
        public string LastBarcodeType { get; set; }

        public event EventHandler GotBarcode;
        public event EventHandler Cleared;
        public event EventHandler Saved;
        public event EventHandler CameraAllowed;
    }
}

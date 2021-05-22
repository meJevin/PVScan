using MvvmHelpers;
using PVScan.Mobile.DAL;
using PVScan.Mobile.Models;
using PVScan.Mobile.Models.API;
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
        readonly IBarcodesRepository BarcodesRepository;
        readonly IFileBarcodeReader FileBarcodeReader;
        readonly IPopupMessageService PopupMessageService;
        readonly IPVScanAPI PVScanAPI;
        readonly IAPIBarcodeHub BarcodeHub;

        public ScanPageViewModel(IBarcodesRepository barcodesRepository,
            IFileBarcodeReader fileBarcodeReader,
            IPopupMessageService popupMessageService,
            IPVScanAPI pVScanAPI, 
            IAPIBarcodeHub barcodeHub)
        {
            BarcodesRepository = barcodesRepository;
            FileBarcodeReader = fileBarcodeReader;
            PopupMessageService = popupMessageService;
            PVScanAPI = pVScanAPI;
            BarcodeHub = barcodeHub;

            BarcodeHub.OnScanned += BarcodeHub_OnScanned;

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
                    ScanTime = DateTime.UtcNow,
                    ScanLocation = null,
                    Favorite = false,
                };

                if (location != null)
                {
                    b.ScanLocation = new Coordinate()
                    {
                        Latitude = location.Latitude,
                        Longitude = location.Longitude,
                    };
                }

                b = await BarcodesRepository.Save(b);

                Saved?.Invoke(this, new EventArgs());

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
                };

                await PVScanAPI.ScannedBarcode(req);
                await BarcodeHub.Scanned(req);
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

            PickPhotoToScanCommand = new Command(async () =>
            {
                var pickResult = await MediaPicker.PickPhotoAsync(new MediaPickerOptions()
                {
                    Title = "Pick a photo which contans barcode",
                });

                if (pickResult == null)
                {
                    return;
                }

                var photoPath = pickResult.FullPath;

                var result = await FileBarcodeReader.DecodeAsync(photoPath);

                if (result == null)
                {
                    _ = PopupMessageService.ShowMessage("No barcodes detected!");
                    return;
                }

                ScanCommand.Execute(result);
            });

            ToggleTorchCommand = new Command(() =>
            {
                TorchEnabled = !TorchEnabled;
            });

            IsCameraAllowed = Permissions.CheckStatusAsync<Permissions.Camera>()
                .GetAwaiter().GetResult() == PermissionStatus.Granted;

            MessagingCenter.Subscribe(this, nameof(CameraAllowedMessage),
                async (ApplicationSettingsPageViewModel v, CameraAllowedMessage args) =>
                {
                    IsCameraAllowed = true;
                });
        }

        private async void BarcodeHub_OnScanned(object sender, ScannedBarcodeRequest b)
        {
            Barcode newBarcode = new Barcode()
            {
                Favorite = b.Favorite,
                Format = b.Format,
                GUID = b.GUID,
                Hash = b.Hash,
                ScanLocation = new Coordinate()
                {
                    Latitude = b.Latitude,
                    Longitude = b.Longitude
                },
                ScanTime = b.ScanTime,
                Text = b.Text,
            };

            newBarcode = await BarcodesRepository.Save(newBarcode);

            MessagingCenter.Send(this, nameof(BarcodeScannedMessage), new BarcodeScannedMessage()
            {
                ScannedBarcode = newBarcode,
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


        public ICommand PickPhotoToScanCommand { get; }
        public ICommand ToggleTorchCommand { get; }
        public bool TorchEnabled { get; set; }
    }
}

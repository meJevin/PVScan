using PVScan.Mobile.ViewModels;
using PVScan.Mobile.ViewModels.Messages.Scanning;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using ZXing;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;
using PVScan.Mobile.Views.Extensions;

namespace PVScan.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScanPage : ContentView
    {
        ZXingScannerView ScannerView;
        ScanPageViewModel VM;

        public ScanPage()
        {
            InitializeComponent();

            VM = (BindingContext as ScanPageViewModel);

            VM.ClearCommand.Execute(null);

            VM.GotBarcode += (s, e) => { BarcodeAvailable(); };
            VM.Cleared += (s, e) => { BarcodeUnavailable(); };
            VM.Saved += (s, e) => {  };
            VM.CameraAllowed += (s, e) => { CameraAllowedHandler(); };

            VM.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(VM.TorchEnabled))
                {
                    ScannerView.IsTorchOn = VM.TorchEnabled;
                }
            };

            if (VM.IsCameraAllowed)
            {
                ScannerView = new ZXingScannerView()
                {
                    IsAnalyzing = true,
                    IsScanning = true,
                };

                ScannerView.Options.CameraResolutionSelector = (res) =>
                {
                    return res.Best();
                };

                ScannerView.OnScanResult += ScannerView_OnScanResult;

                ScannerViewContainer.Children.Clear();
                ScannerViewContainer.Children.Add(ScannerView);
            }

            //MessagingCenter.Subscribe(this, nameof(CameraAllowedMessage),
            //    async (ApplicationSettingsPageViewModel v, CameraAllowedMessage args) =>
            //    {
            //        CameraAllowedHandler();
            //    });
        }

        private void BarcodeAvailable()
        {
            _ = BarcodeInfoContainer.TranslateTo(0, 0, 250, Easing.CubicOut);
            _ = SaveButtonContainer.TranslateTo(0, 0, 250, Easing.CubicOut);
            _ = PhotoAndTorchContainer.TranslateTo(0, -SaveButtonContainer.Height, 250, Easing.CubicOut);
            _ = PhotoAndTorchContainer.MarginBottomTo(0, 250, Easing.CubicOut);
        }

        private void BarcodeUnavailable()
        {
            _ = BarcodeInfoContainer.TranslateTo(0, -BarcodeInfoContainer.Height, 250, Easing.CubicOut);
            _ = SaveButtonContainer.TranslateTo(0, SaveButtonContainer.Height, 250, Easing.CubicOut);
            _ = PhotoAndTorchContainer.TranslateTo(0, 0, 250, Easing.CubicOut);
            _ = PhotoAndTorchContainer.MarginBottomTo(40, 250, Easing.CubicOut);
        }

        public async Task Initialize()
        {
            if (ScannerView == null)
            {
                return;
            }

            ScannerView.IsScanning = true;
            ScannerView.IsAnalyzing = true;

            ScannerView.IsTorchOn = VM.TorchEnabled;
        }

        public async Task Uninitialize()
        {
            if (ScannerView == null)
            {
                return;
            }

            ScannerView.IsAnalyzing = false;
            ScannerView.IsTorchOn = false;

            // Remove this comment if you want slower tab switch speed
            // for scan page. But camra is actually turned off here
            // and the device is going to be faster
            //ScannerView.IsScanning = false;
        }

        private async Task CameraAllowedHandler()
        {
            ScannerView = new ZXingScannerView()
            {
                IsAnalyzing = true,
                IsScanning = true,
            };

            ScannerView.Options.CameraResolutionSelector = (res) =>
            {
                return res.Best();
            };

            ScannerView.OnScanResult += ScannerView_OnScanResult;

            ScannerViewContainer.Children.Clear();
            ScannerViewContainer.Children.Add(ScannerView);

            // This hack is required unfortunatelly :(
            _ = Task.Run(async () => 
            {
                await Task.Delay(5);

                Device.BeginInvokeOnMainThread(() =>
                {
                    ScannerViewContainer.Children.Clear();
                    ScannerViewContainer.Children.Add(ScannerView);
                });
            });
        }

        private void ScannerView_OnScanResult(Result result)
        {
            (BindingContext as ScanPageViewModel).ScanCommand.Execute(result);
        }

        void TorchButton_Clicked(object sender, EventArgs e)
        {
            VM.TorchEnabled = !VM.TorchEnabled;
        }

        // The only way i found to set their TranslationY on stratup
        #region Container Initiialization
        private void BarcodeInfoContainer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Height))
            {
                BarcodeInfoContainer.TranslationY = -BarcodeInfoContainer.Height;
            }
        }

        private void SaveButtonContainer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Height))
            {
                SaveButtonContainer.TranslationY = SaveButtonContainer.Height;
            }
        }
        #endregion
    }
}
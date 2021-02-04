using PVScan.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using ZXing;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace PVScan.Mobile.Views
{
    public partial class ScanPage : ContentView
    {
        public ScanPage()
        {
            InitializeComponent();

            ScannerView.Options = new MobileBarcodeScanningOptions()
            {
                TryHarder = true,
                AutoRotate = true,
                PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE, BarcodeFormat.All_1D },
                CameraResolutionSelector = (res) =>
                {
                    return res.Last();
                },
            };

            ScannerView.OnScanResult += ScannerView_OnScanResult;

            var vm = (BindingContext as ScanPageViewModel);

            vm.ClearCommand.Execute(null);

            vm.GotBarcode += (s, e) => { BarcodeAvailable(); };
            vm.Cleared += (s, e) => { BarcodeUnavailable(); };
            vm.Saved += (s, e) => {  };
        }

        private void BarcodeAvailable()
        {
            _ = BarcodeInfoContainer.TranslateTo(0, 0, 250, Easing.CubicOut);
            _ = SaveButtonContainer.TranslateTo(0, 0, 250, Easing.CubicOut);
        }

        private void BarcodeUnavailable()
        {
            _ = BarcodeInfoContainer.TranslateTo(0, -BarcodeInfoContainer.Height, 250, Easing.CubicOut);
            _ = SaveButtonContainer.TranslateTo(0, SaveButtonContainer.Height, 250, Easing.CubicOut);
        }

        public async Task Initialize()
        {
            ScannerView.IsScanning = true;
            ScannerView.IsAnalyzing = true;
        }

        public async Task Uninitialize()
        {
            ScannerView.IsScanning = false;
            ScannerView.IsAnalyzing = false;
        }

        private void ScannerView_OnScanResult(Result result)
        {
            (BindingContext as ScanPageViewModel).ScanCommand.Execute(result);
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace PVScan.Mobile.Views
{
    public partial class ScanPage : ContentPage
    {
        ZXingScannerView ScannerView;

        public ScanPage()
        {
            InitializeComponent();

            ScannerView = new ZXingScannerView()
            {
                Options = new MobileBarcodeScanningOptions()
                {
                    TryHarder = true,
                    AutoRotate = true,
                    PossibleFormats = new List<ZXing.BarcodeFormat> { ZXing.BarcodeFormat.QR_CODE, ZXing.BarcodeFormat.All_1D },
                    CameraResolutionSelector = (res) =>
                    {
                        return res.Last();
                    },
                },
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                AutomationId = "zxingScannerView",
            };

            ScannerView.OnScanResult += ScannerView_OnScanResult;

            MainContainer.Children.Add(ScannerView);
        }

        protected override void OnAppearing()
        {
            ScannerView.IsScanning = true;
            ScannerView.IsAnalyzing = true;
        }

        protected override void OnDisappearing()
        {
            ScannerView.IsScanning = false;
            ScannerView.IsAnalyzing = false;
        }

        private void ScannerView_OnScanResult(ZXing.Result result)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                // Stop analysis until we navigate away so we don't keep reading barcodes
                ScannerView.IsAnalyzing = false;

                // Show an alert
                await DisplayAlert("Scanned Barcode", result.Text, "OK");

                ScannerView.IsAnalyzing = true;
            });
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
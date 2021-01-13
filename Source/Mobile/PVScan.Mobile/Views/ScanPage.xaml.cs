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

namespace PVScan.Mobile.Views
{
    public partial class ScanPage : ContentPage
    {
        public ScanPage()
        {
            InitializeComponent();

            PropertyChanged += ContentPageBase_PropertyChanged;
        }

        private void ContentPageBase_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        protected override void OnAppearing()
        {
            ScannerView.IsAnalyzing = true;
            ScannerView.IsScanning = true;
        }

        protected override void OnDisappearing()
        {
            ScannerView.IsAnalyzing = false;
            ScannerView.IsScanning = false;
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
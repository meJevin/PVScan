using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Mobile;

namespace PVScan.Mobile.Views
{
    public partial class ScanPage : ContentPage
    {
        public ScanPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            ScannerView.IsAnalyzing = true;
            ScannerView.IsScanning = true;
            ScannerView.ToggleTorch();
        }

        protected override void OnDisappearing()
        {
            ScannerView.IsAnalyzing = false;
            ScannerView.IsScanning = false;
            ScannerView.ToggleTorch();
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
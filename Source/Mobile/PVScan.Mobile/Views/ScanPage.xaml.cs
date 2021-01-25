using PVScan.Mobile.ViewModels;
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

            (BindingContext as ScanPageViewModel).ClearCommand.Execute(null);
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
    }
}
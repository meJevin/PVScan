﻿using System;
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

        public async Task Initialize()
        {
            ScannerView.IsAnalyzing = true;
            ScannerView.IsScanning = true;
        }

        public async Task Uninitialize()
        {
            ScannerView.IsAnalyzing = false;
            ScannerView.IsScanning = false;
        }

        private async void ScannerView_OnScanResult(ZXing.Result result)
        {
            // Stop analysis until we navigate away so we don't keep reading barcodes
            ScannerView.IsAnalyzing = false;

            // Show an alert
            Console.WriteLine($"\n\nSCANNED: {result.Text} {Enum.GetName(typeof(BarcodeFormat), result.BarcodeFormat)}");

            ScannerView.IsAnalyzing = true;
        }
    }
}
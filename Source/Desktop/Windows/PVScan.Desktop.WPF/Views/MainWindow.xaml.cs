﻿using PVScan.Core;
using PVScan.Desktop.WPF.ViewModels;
using PVScan.Desktop.WPF.ViewModels.Messages.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PVScan.Desktop.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel VM;

        double BarcodeInfoPageHeight = -1;
        bool showingBarcodeInfoPage = false;

        public MainWindow()
        {
            InitializeComponent();

            VM = DataContext as MainWindowViewModel;
            VM.MapScanPagesToggled += VM_MapScanPagesToggled;

            BarcodeInfoPage.SizeChanged += async (_, _) =>
            {
                if (BarcodeInfoPage.ActualHeight != BarcodeInfoPageHeight &&
                    !showingBarcodeInfoPage)
                {
                    BarcodeInfoPageHeight = BarcodeInfoPage.ActualHeight;
                    await HideBarcodeInfoPage(TimeSpan.Zero);
                }
            };

            _ = ToggleToMapPage(TimeSpan.Zero);
        }

        private async Task HideBarcodeInfoPage(TimeSpan duration)
        {
            await BarcodeInfoPage.TranslateTo(0, BarcodeInfoPage.ActualHeight, duration);
            showingBarcodeInfoPage = false;
        }

        private async Task ShowBarcodeInfoPage(TimeSpan duration)
        {
            await BarcodeInfoPage.TranslateTo(0, 0, duration);
            showingBarcodeInfoPage = true;
        }

        private async void VM_MapScanPagesToggled(object sender, EventArgs e)
        {
            if (ToggleScanMapPagesButton.Content.ToString() == "Map")
            {
                await ToggleToMapPage(Animations.DefaultDuration);
            }
            else
            {
                await ToggleToScanPage(Animations.DefaultDuration);
            }
        }

        private async Task ToggleToMapPage(TimeSpan duration)
        {
            ToggleScanMapPagesButton.Content = "Scan";
            MapPageView.Visibility = Visibility.Visible;

            ScanPageView.IsHitTestVisible = false;
            _ = ScanPageView.FadeTo(0, duration);

            MapPageView.IsHitTestVisible = true;
            await MapPageView.FadeTo(1, duration);

            ScanPageView.Visibility = Visibility.Hidden;

            (ScanPageView.DataContext as ScanPageViewModel).StopCapturing();
        }

        private async Task ToggleToScanPage(TimeSpan duration)
        {
            ToggleScanMapPagesButton.Content = "Map";
            ScanPageView.Visibility = Visibility.Visible;

            MapPageView.IsHitTestVisible = false;
            _ = MapPageView.FadeTo(0, duration);

            ScanPageView.IsHitTestVisible = true;
            await ScanPageView.FadeTo(1, duration);

            MapPageView.Visibility = Visibility.Hidden;

            (ScanPageView.DataContext as ScanPageViewModel).StartCapturing();
        }

        private async void MapPageView_BarcodeSelected(object sender, Core.Models.Barcode e)
        {
            (BarcodeInfoPage.DataContext as BarcodeInfoPageViewModel).SelectedBarcode = e;
            await ShowBarcodeInfoPage(Animations.DefaultDuration);
        }

        private async void HistoryPage_BarcodeSelected(object sender, Core.Models.Barcode e)
        {
            (BarcodeInfoPage.DataContext as BarcodeInfoPageViewModel).SelectedBarcode = e;
            await ShowBarcodeInfoPage(Animations.DefaultDuration);
        }

        private async void BarcodeInfoPage_Closed(object sender, EventArgs e)
        {
            var HPVM = (HistoryPage.DataContext as HistoryPageViewModel);

            if (!HPVM.IsEditing)
            {
                HPVM.SelectedBarcode = null;
            }

            await HideBarcodeInfoPage(Animations.DefaultDuration);
        }

        private void PopupOverlay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessagingCenter.Send(this, nameof(PopupDismissedViaOverlayMessage),
                new PopupDismissedViaOverlayMessage() { });
        }
    }
}

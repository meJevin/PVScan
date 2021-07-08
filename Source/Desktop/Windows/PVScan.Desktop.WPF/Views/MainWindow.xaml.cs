using PVScan.Core;
using PVScan.Core.Models;
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
        bool showingProfilePage = false;

        public MainWindow()
        {
            InitializeComponent();

            VM = DataContext as MainWindowViewModel;
            VM.MapScanPagesToggled += VM_MapScanPagesToggled;
            VM.LocationSpecificationStarted += VM_LocationSpecificationStarted;

            BarcodeInfoPage.SizeChanged += async (_, _) =>
            {
                if (BarcodeInfoPage.ActualHeight != BarcodeInfoPageHeight &&
                    !showingBarcodeInfoPage)
                {
                    BarcodeInfoPageHeight = BarcodeInfoPage.ActualHeight;
                    await HideBarcodeInfoPage(TimeSpan.Zero);
                }
            };

            (BarcodeInfoPage.DataContext as BarcodeInfoPageViewModel).Closed += BarcodeInfoPage_Closed;

            _ = ToggleToMapPage(TimeSpan.Zero);
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

        private void VM_LocationSpecificationStarted(object sender, Barcode e)
        {
            var mapPageVM = (MapPageView.DataContext as MapPageViewModel);

            mapPageVM.IsSpecifyingLocation = true;
            mapPageVM.LocationSpecificationBarcode = e;

            var HPVM = (HistoryPage.DataContext as HistoryPageViewModel);

            HPVM.SelectedBarcode = null;
            (BarcodeInfoPage.DataContext as BarcodeInfoPageViewModel).SelectedBarcode = null;

            _ = HideBarcodeInfoPage(Animations.DefaultDuration);
        }

        private async Task ShowProfilePage(TimeSpan duration)
        {
            _ = ProfilePageContainer.FadeTo(1, duration);
            ProfilePageOverlay.IsHitTestVisible = true;
            _ = ProfilePageOverlay.FadeTo(0.85, duration);
            await ProfilePage.TranslateTo(0, 0, duration);
            ProfilePageContainer.IsHitTestVisible = true;
            showingProfilePage = true;
        }

        private async Task HideProfilePage(TimeSpan duration)
        {
            ProfilePageContainer.IsHitTestVisible = false;
            _ = ProfilePageContainer.FadeTo(0, duration);
            _ = ProfilePageOverlay.FadeTo(0, duration);
            await ProfilePage.TranslateTo(ProfilePage.ActualWidth, 0, duration);
            ProfilePageOverlay.IsHitTestVisible = false;
            showingProfilePage = false;
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

        private void PopupOverlay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessagingCenter.Send(this, nameof(PopupDismissedViaOverlayMessage),
                new PopupDismissedViaOverlayMessage() { });
        }

        private async void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            await ShowProfilePage(Animations.DefaultDuration);
        }

        private async void ProfilePageOverlay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            await HideProfilePage(Animations.DefaultDuration);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await ProfilePage.Initialize();
            VM.LoadedCommand.Execute(null);
        }
    }
}

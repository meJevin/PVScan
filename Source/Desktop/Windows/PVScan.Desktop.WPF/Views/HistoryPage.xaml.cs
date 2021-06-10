using PVScan.Core;
using PVScan.Core.Models;
using PVScan.Desktop.WPF.ViewModels;
using PVScan.Desktop.WPF.ViewModels.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
    /// Interaction logic for HistoryPage.xaml
    /// </summary>
    public partial class HistoryPage : ContentControl
    {
        double SortingPageHeight = -1;
        double FilterPageHeight = -1;
        double BarcodeInfoPageHeight = -1;
        double SearchDelay = 500;
        Timer SearchDelayTimer;

        HistoryPageViewModel VM;

        double OverlayMaxOpacity = 0.65;

        public HistoryPage()
        {
            InitializeComponent();

            SearchDelayTimer = new Timer(SearchDelay);
            SearchDelayTimer.Enabled = false;
            SearchDelayTimer.Elapsed += SearchDelayTimer_Elapsed;
            SearchDelayTimer.AutoReset = false;

            VM = DataContext as HistoryPageViewModel;

            SortingPage.SizeChanged += async (_, _) =>
            {
                if (SortingPage.ActualHeight != SortingPageHeight &&
                    SortingPageOverlay.Opacity != OverlayMaxOpacity)
                {
                    SortingPageHeight = SortingPage.ActualHeight;
                    await HideSortingPage(TimeSpan.Zero);
                }
            };

            FilterPage.SizeChanged += async (_, _) =>
            {
                if (FilterPage.ActualHeight != FilterPageHeight &&
                    FilterPageOverlay.Opacity != OverlayMaxOpacity)
                {
                    FilterPageHeight = FilterPage.ActualHeight;
                    await HideFilterPage(TimeSpan.Zero);
                }
            };

            BarcodeInfoPage.SizeChanged += async (_, _) =>
            {
                if (BarcodeInfoPage.ActualHeight != BarcodeInfoPageHeight &&
                    BarcodeInfoPageOverlay.Opacity != OverlayMaxOpacity)
                {
                    BarcodeInfoPageHeight = BarcodeInfoPage.ActualHeight;
                    await HideBarcodeInfoPage(TimeSpan.Zero);
                }
            };

            VM.PropertyChanged += VM_PropertyChanged;
        }

        private async void VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VM.IsEditing))
            {
                if (VM.IsEditing)
                {
                    LoadedBarcodesListView.SelectionMode = SelectionMode.Multiple;

                    _ = StartEditButton.FadeTo(0, Animations.DefaultDuration);
                    StartEditButton.IsHitTestVisible = false;
                    _ = DoneEditButton.FadeTo(1, Animations.DefaultDuration);
                    await DeleteButton.FadeTo(1, Animations.DefaultDuration);
                }
                else
                {
                    LoadedBarcodesListView.SelectionMode = SelectionMode.Single;

                    _ = StartEditButton.FadeTo(1, Animations.DefaultDuration);
                    StartEditButton.IsHitTestVisible = true;
                    _ = DoneEditButton.FadeTo(0, Animations.DefaultDuration);
                    await DeleteButton.FadeTo(0, Animations.DefaultDuration);
                }
            }
        }

        private void SearchDelayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                VM.SearchCommand.Execute(null);
            });
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchDelayTimer.Interval = SearchDelay;
            SearchDelayTimer.Enabled = true;
        }

        private void BarcodesScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (BarecodesScrollViewer.ScrollableHeight < 50)
            {
                return;
            }

            if (e.VerticalOffset >= BarecodesScrollViewer.ScrollableHeight - 50)
            {
                VM.LoadNextPage.Execute(null);
            }
        }

        private void BarecodesScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private async Task HideSortingPage(TimeSpan duration)
        {
            SortingPageOverlay.IsHitTestVisible = false;

            _ = SortingPage.TranslateTo(0, SortingPage.ActualHeight, duration);
            await SortingPageOverlay.FadeTo(0, duration);
        }

        private async Task ShowSortingPage(TimeSpan duration)
        {
            SortingPageOverlay.IsHitTestVisible = true;

            _ = SortingPageOverlay.FadeTo(OverlayMaxOpacity, duration);
            await SortingPage.TranslateTo(0, 0, duration);
        }

        private async void SortingPageOverlay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            await HideSortingPage(Animations.DefaultDuration);
        }

        private async Task HideFilterPage(TimeSpan duration)
        {
            FilterPageOverlay.IsHitTestVisible = false;

            _ = FilterPage.TranslateTo(0, FilterPage.ActualHeight, duration);
            await FilterPageOverlay.FadeTo(0, duration);
        }

        private async Task ShowFilterPage(TimeSpan duration)
        {
            FilterPageOverlay.IsHitTestVisible = true;

            _ = FilterPageOverlay.FadeTo(OverlayMaxOpacity, duration);
            await FilterPage.TranslateTo(0, 0, duration);
        }

        private async void FilterPageOverlay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            await HideFilterPage(Animations.DefaultDuration);
        }

        private async void SortingButton_Click(object sender, RoutedEventArgs e)
        {
            await ShowSortingPage(Animations.DefaultDuration);
        }

        private async void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            await ShowFilterPage(Animations.DefaultDuration);
        }

        private async Task HideBarcodeInfoPage(TimeSpan duration)
        {
            BarcodeInfoPageOverlay.IsHitTestVisible = false;

            _ = BarcodeInfoPage.TranslateTo(0, BarcodeInfoPage.ActualHeight, duration);
            await BarcodeInfoPageOverlay.FadeTo(0, duration);
        }

        private async Task ShowBarcodeInfoPage(TimeSpan duration)
        {
            BarcodeInfoPageOverlay.IsHitTestVisible = true;

            _ = BarcodeInfoPageOverlay.FadeTo(OverlayMaxOpacity, duration);
            await BarcodeInfoPage.TranslateTo(0, 0, duration);
        }

        private async void BarcodeInfoPageOverlay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            await HideBarcodeInfoPage(Animations.DefaultDuration);

            LoadedBarcodesListView.SelectionChanged -= LoadedBarcodesListView_SelectionChanged;
            LoadedBarcodesListView.SelectedItem = null;
            LoadedBarcodesListView.SelectionChanged += LoadedBarcodesListView_SelectionChanged;
        }

        private async void LoadedBarcodesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VM.IsEditing)
            {
                return;
            }

            if ((sender as ListView).SelectedItem == null)
            {
                return;
            }

            (BarcodeInfoPage.DataContext as BarcodeInfoPageViewModel).SelectedBarcode 
                = (sender as ListView).SelectedItem as Barcode;

            // Todo: this is some weird bug with WPF
            await ShowBarcodeInfoPage(Animations.DefaultDuration);
            await ShowBarcodeInfoPage(Animations.DefaultDuration);
        }
    }
}

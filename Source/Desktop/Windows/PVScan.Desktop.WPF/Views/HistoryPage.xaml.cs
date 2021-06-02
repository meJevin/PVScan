using PVScan.Core;
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
                if (SortingPage.ActualHeight != SortingPageHeight)
                {
                    SortingPageHeight = SortingPage.ActualHeight;
                    await HideSortingPage(TimeSpan.Zero);
                }
            };
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

        private async void SortingButton_Click(object sender, RoutedEventArgs e)
        {
            await ShowSortingPage(Animations.DefaultDuration);
        }

        private async void SortingPageOverlay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            await HideSortingPage(Animations.DefaultDuration);
        }
    }
}

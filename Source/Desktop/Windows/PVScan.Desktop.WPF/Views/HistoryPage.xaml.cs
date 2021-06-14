using PVScan.Core;
using PVScan.Core.Models;
using PVScan.Desktop.WPF.ViewModels;
using PVScan.Desktop.WPF.ViewModels.Messages;
using PVScan.Desktop.WPF.ViewModels.Messages.Barcodes;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
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
        double SearchDelay = 500;
        Timer SearchDelayTimer;

        HistoryPageViewModel VM;

        double OverlayMaxOpacity = 0.65;

        public event EventHandler<Barcode> BarcodeSelected;

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

            VM.PropertyChanged += VM_PropertyChanged;
            VM.SelectedBarcodes.CollectionChanged += SelectedBarcodes_CollectionChanged;

            MessagingCenter.Subscribe(this, nameof(ShowBarcodeInListMessage),
                async (MainWindowViewModel vm, ShowBarcodeInListMessage args) =>
                {
                    while (!VM.BarcodesPaged.Contains(args.BarcodeToShow))
                    {
                        VM.LoadNextPage.Execute(null);
                    }

                    await Task.Delay(250);

                    var desiredItem = LoadedBarcodesListView
                                            .ItemContainerGenerator
                                            .ContainerFromItem(args.BarcodeToShow) as UIElement;

                    var desiredItemIndex = LoadedBarcodesListView
                                                .ItemContainerGenerator
                                                .IndexFromContainer(desiredItem, true);

                    var viewportHeight = BarecodesScrollViewer.RenderSize.Height;

                    var itemHeight = desiredItem.RenderSize.Height;

                    var topOffset = itemHeight * desiredItemIndex;
                    topOffset -= viewportHeight / 2;

                    BarecodesScrollViewer.ScrollToVerticalOffset(topOffset);
                });
        }

        private async void SelectedBarcodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!VM.IsEditing)
            {
                return;
            }

            if (VM.SelectedBarcodes.Count == 0)
            {
                DeleteButton.IsHitTestVisible = false;
                await DeleteButton.FadeTo(0.5, Animations.DefaultDuration);
            }
            else
            {
                DeleteButton.IsHitTestVisible = true;
                await DeleteButton.FadeTo(1, Animations.DefaultDuration);
            }
        }

        private async void VM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VM.IsEditing))
            {
                if (VM.IsEditing)
                {
                    LoadedBarcodesListView.SelectedItem = null;
                    LoadedBarcodesListView.SelectionMode = SelectionMode.Multiple;

                    _ = StartEditButton.FadeTo(0, Animations.DefaultDuration);
                    StartEditButton.IsHitTestVisible = false;
                    _ = DoneEditButton.FadeTo(1, Animations.DefaultDuration);
                    await DeleteButton.FadeTo(0.5, Animations.DefaultDuration);
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

            BarcodeSelected?.Invoke(this, (sender as ListView).SelectedItem as Barcode);
        }
    }
}

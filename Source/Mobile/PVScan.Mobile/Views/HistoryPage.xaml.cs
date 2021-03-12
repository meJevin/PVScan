using PVScan.Mobile.ViewModels;
using PVScan.Mobile.ViewModels.Messages.Filtering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
using PVScan.Mobile.Views.Extensions;
using PVScan.Mobile.Effects;
using PVScan.Mobile.Models;
using PVScan.Mobile.Views.DataTemplates;
using System.Collections.Specialized;

namespace PVScan.Mobile.Views
{
    // IMPORTANT
    // I use 1px offsets when translating UI elements because on Anroid sometimes UI elements don't line up
    // like they do on iOS. So this little hack is fine, I guess ;)
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoryPage : ContentView
    {
        double FilterPageHeight;
        double OverlayMaxOpacity = 0.65;

        double SearchDelay = 500;
        Timer SearchDelayTimer;

        bool FilterBarHidden;

        // Cancel event flag for when we long press a barcode
        bool CancelBarcodeTapped = false;

        HistoryPageViewModel VM;

        public HistoryPage()
        {
            InitializeComponent();

            LayoutChanged += async (s, e) =>
            {
                _ = ShowListView(0);
                _ = HideFilterBar(0);
                _ = HideFilterView(0);
                _ = HideCopyToClipboardNotification(0);
                _ = HideBarcodeInfo(0);
            };

            SearchDelayTimer = new Timer(SearchDelay);
            SearchDelayTimer.Enabled = false;
            SearchDelayTimer.Elapsed += SearchDelayTimer_Elapsed;
            SearchDelayTimer.AutoReset = false;

            FilterPageOverlay.Opacity = 0;
            FilterPageOverlay.InputTransparent = true;

            if (Device.RuntimePlatform == Device.Android)
            {
                BarcodesRefreshView.RefreshColor = Color.Black;
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                BarcodesRefreshView.SetAppThemeColor(RefreshView.RefreshColorProperty, Color.Black, Color.White);
            }

            VM = (BindingContext as HistoryPageViewModel);

            VM.BarcodeCopiedToClipboard += HistoryPage_BarcodeCopiedToClipboard;
            VM.PropertyChanged += VM_PropertyChanged;
            VM.SelectedBarcodes.CollectionChanged += SelectedBarcodes_CollectionChanged;

            InitializeNormalBarcodeItemTemplate();

            MessagingCenter.Subscribe(this, nameof(FilterAppliedMessage),
                async (FilterPageViewModel vm, FilterAppliedMessage args) =>
                {
                    await HideFilterView();
                });
        }


        private bool Initialized = false;
        public async Task Initialize()
        {
            if (Initialized) return;

            await (BindingContext as HistoryPageViewModel).LoadBarcodesFromDB();

            try
            {
                var initialLocation = await Geolocation.GetLocationAsync(new GeolocationRequest()
                {
                    DesiredAccuracy = GeolocationAccuracy.Best,
                    Timeout = TimeSpan.FromSeconds(1.5),
                });

                Map.MoveToRegion(MapSpan.FromCenterAndRadius(
                    new Position(initialLocation.Latitude, initialLocation.Longitude),
                    Distance.FromKilometers(0.5)));
            }
            catch
            {

            }

            Initialized = true;
        }

        private void InitializeNormalBarcodeItemTemplate()
        {
            BarcodesCollectionView.ItemTemplate = new DataTemplate(() =>
            {
                var item = new NormalBarcodeItem();
                item.Tapped += Barcode_Tapped;
                return item;
            });
        }

        private void InitializeSelectableBarcodeItemTemplate()
        {
            BarcodesCollectionView.ItemTemplate = new DataTemplate(() =>
            {
                var item = new SelectableBarcodeItem();
                return item;
            });
        }


        private async void SelectedBarcodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (VM.IsEditing)
            {
                if (VM.SelectedBarcodes.Count == 0)
                {
                    if (!DeleteButton.InputTransparent)
                    {
                        DeleteButton.FadeTo(0.5, 250, Easing.CubicOut);
                        DeleteButton.InputTransparent = true;
                    }
                }
                else
                {
                    if (DeleteButton.InputTransparent)
                    {
                        DeleteButton.FadeTo(1, 250, Easing.CubicOut);
                        DeleteButton.InputTransparent = false;
                    }
                }
            }
        }

        private async void VM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VM.IsEditing))
            {
                if (VM.IsEditing)
                {
                    EditButton.InputTransparent = true;

                    _ = DeleteButton.FadeTo(0.5, 250, Easing.CubicOut);
                    _ = DoneButton.FadeTo(1, 250, Easing.CubicOut);
                    _ = await EditButton.FadeTo(0, 250, Easing.CubicOut);

                    DoneButton.InputTransparent = false;

                    InitializeSelectableBarcodeItemTemplate();
                    BarcodesCollectionView.SelectionMode = SelectionMode.Multiple;
                }
                else
                {
                    DoneButton.InputTransparent = true;
                    DeleteButton.InputTransparent = true;

                    _ = DeleteButton.FadeTo(0, 250, Easing.CubicOut);
                    _ = DoneButton.FadeTo(0, 250, Easing.CubicOut);
                    _ = await EditButton.FadeTo(1, 250, Easing.CubicOut);

                    EditButton.InputTransparent = false;

                    InitializeNormalBarcodeItemTemplate();
                    BarcodesCollectionView.SelectionMode = SelectionMode.None;
                }
            }
        }

        private async void HistoryPage_BarcodeCopiedToClipboard(object sender, Barcode e)
        {
            CancelBarcodeTapped = true;

            await ShowCopyToClipboardNotification(250);

            await Task.Delay(1000);

            await HideCopyToClipboardNotification(250);
        }

        private async Task ShowCopyToClipboardNotification(uint duration)
        {
            await CopiedToClipboardNotification.TranslateTo(0, 1, duration, Easing.CubicOut);
        }

        private async Task HideCopyToClipboardNotification(uint duration)
        {
            await CopiedToClipboardNotification.TranslateTo(0, CopiedToClipboardNotification.Height + 1, duration, Easing.CubicOut);
        }

        private async void FilterViewPanGesture_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (e.StatusType == GestureStatus.Running)
            {
                double newTranslationY = FilterPage.TranslationY;

                // Because xamarin :L
                // The interesting thing is that values are actually the same, but android does something weird and we actually have to add, IDK
                if (Device.RuntimePlatform == Device.Android)
                {
                    newTranslationY += e.TotalY;
                }
                else if (Device.RuntimePlatform == Device.iOS)
                {
                    newTranslationY = e.TotalY;
                }

                if (newTranslationY < 1)
                {
                    newTranslationY = 1;
                }

                double newOverlayOpacity = OverlayMaxOpacity - ((newTranslationY / FilterPageHeight) * OverlayMaxOpacity);

                FilterPage.TranslationY = newTranslationY;
                FilterPageOverlay.Opacity = newOverlayOpacity;

                //Console.WriteLine($"\nTOTAL_T, NEW_TRANS_Y: {e.TotalY}, {newTranslationY}");
            }
            else if (e.StatusType == GestureStatus.Completed)
            {
                if (FilterPage.TranslationY < (FilterPageHeight * 0.25))
                {
                    await ShowFilterView();
                }
                else
                {
                    await HideFilterView();
                }
            }
        }

        private async void FilterButton_Clicked(object sender, EventArgs e)
        {
            await ShowFilterView();
        }

        private async void FilterPageOverlay_Tapped(object sender, EventArgs e)
        {
            await HideFilterView();
        }

        private async Task HideFilterView(uint duration = 250)
        {
            FilterPageOverlay.InputTransparent = true;

            _ = FilterPageOverlay.FadeTo(0, duration, Easing.CubicOut);
            await FilterPage.TranslateTo(0, FilterPageHeight, duration, Easing.CubicOut);
        }

        private async Task ShowFilterView(uint duration = 250)
        {
            FilterPageOverlay.InputTransparent = false;

            _ = FilterPageOverlay.FadeTo(OverlayMaxOpacity, duration, Easing.CubicOut);
            await FilterPage.TranslateTo(0, 1, duration, Easing.CubicOut);
        }

        private void SearchDelayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                (BindingContext as HistoryPageViewModel).SearchCommand.Execute(null);
            });
        }

        private void SearchEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchDelayTimer.Interval = SearchDelay;
            SearchDelayTimer.Enabled = true;
        }

        private async Task ShowFilterBar(uint duration = 250)
        {
            _ = FilterBarContainer.TranslateTo(0, 0, duration, Easing.CubicOut);
            _ = ShowHideFilterBarButton.FadeTo(1, duration, Easing.CubicOut);
            _ = ShowHideFilterBarButton.TranslateTo(0, ShowHideFilterBarButton.Height / -2, duration, Easing.CubicOut);

            _ = BarcodesListContainer.PaddingTopTo(FilterBar.Height, duration, Easing.CubicOut);

            await ShowHideFilterButtonImage.RotateTo(180, duration, Easing.CubicOut);

            FilterBarHidden = false;
        }

        private async Task HideFilterBar(uint duration = 250)
        {
            _ = FilterBarContainer.TranslateTo(0, -FilterBar.Height, duration, Easing.CubicOut);
            _ = ShowHideFilterBarButton.FadeTo(0.5, duration, Easing.CubicOut);
            _ = ShowHideFilterBarButton.TranslateTo(0, 0, duration, Easing.CubicOut);

            _ = BarcodesListContainer.PaddingTopTo(0, duration, Easing.CubicOut);

            await ShowHideFilterButtonImage.RotateTo(0, duration, Easing.CubicOut);

            FilterBarHidden = true;
        }

        private async void ShowHideFilterBarButton_Clicked(object sender, EventArgs e)
        {
            if (FilterBarHidden)
            {
                await ShowFilterBar();
            }
            else
            {
                await HideFilterBar();
            }
        }

        private async Task ShowListView(uint duration = 250)
        {
            _ = BarcodesListContainer.TranslateTo(0, 0, duration, Easing.CubicOut);
            await MapViewContainer.TranslateTo(MapViewContainer.Width, 0, duration, Easing.CubicOut);

            ListViewButton.Opacity = 1;
            MapViewButton.Opacity = 0.35;
        }

        private async Task ShowMapView(uint duration = 250)
        {
            _ = MapViewContainer.TranslateTo(0, 0, duration, Easing.CubicOut);
            await BarcodesListContainer.TranslateTo(-BarcodesListContainer.Width, 0, duration, Easing.CubicOut);

            MapViewButton.Opacity = 1;
            ListViewButton.Opacity = 0.35;
        }

        private async void ListViewButton_Clicked(object sender, EventArgs e)
        {
            await ShowListView();
        }

        private async void MapViewButton_Clicked(object sender, EventArgs e)
        {
            await ShowMapView();
        }

        // Unfortunatelly the only way I found to size FilterPage properly :(
        private void ContentView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Height))
            {
                FilterPageHeight = Height - 50;
                FilterPage.HeightRequest = FilterPageHeight;
                FilterPage.TranslationY = FilterPageHeight;
            }
        }

        private async void BarcodeInfoOverlay_Tapped(object sender, EventArgs e)
        {
            await HideBarcodeInfo();
        }

        private async Task HideBarcodeInfo(uint duration = 250)
        {
            BarcodeInfoOverlay.InputTransparent = true;

            _ = BarcodeInfoOverlay.FadeTo(0, duration, Easing.CubicOut);
            await BarcodeInfo.TranslateTo(0, BarcodeInfo.Height, duration, Easing.CubicOut);
        }

        private async Task ShowBarcodeInfo(uint duration = 250)
        {
            BarcodeInfoOverlay.InputTransparent = false;

            _ = BarcodeInfoOverlay.FadeTo(OverlayMaxOpacity, duration, Easing.CubicOut);
            await BarcodeInfo.TranslateTo(0, 1, duration, Easing.CubicOut);
        }

        private async void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (e.StatusType == GestureStatus.Running)
            {
                double newTranslationY = BarcodeInfo.TranslationY;

                // Because xamarin :L
                // The interesting thing is that values are actually the same, but android does something weird and we actually have to add, IDK
                if (Device.RuntimePlatform == Device.Android)
                {
                    newTranslationY += e.TotalY;
                }
                else if (Device.RuntimePlatform == Device.iOS)
                {
                    newTranslationY = e.TotalY;
                }

                if (newTranslationY < 1)
                {
                    newTranslationY = 1;
                }

                double newOverlayOpacity = OverlayMaxOpacity - ((newTranslationY / BarcodeInfo.Height) * OverlayMaxOpacity);

                BarcodeInfo.TranslationY = newTranslationY;
                BarcodeInfoOverlay.Opacity = newOverlayOpacity;

                //Console.WriteLine($"\nTOTAL_T, NEW_TRANS_Y: {e.TotalY}, {newTranslationY}");
            }
            else if (e.StatusType == GestureStatus.Completed)
            {
                if (BarcodeInfo.TranslationY < (BarcodeInfo.Height * 0.25))
                {
                    await ShowBarcodeInfo();
                }
                else
                {
                    await HideBarcodeInfo();
                }
            }
        }

        private async void Barcode_Tapped(object sender, EventArgs e)
        {
            var barcodeGrid = sender as Grid;

            if (barcodeGrid == null || CancelBarcodeTapped)
            {
                CancelBarcodeTapped = false;
                return;
            }

            await ShowBarcodeInfo();
        }

        private async void BarcodeInfoShowOnMap_Clicked(object sender, EventArgs e)
        {
            Barcode selectedBarcode = (BindingContext as HistoryPageViewModel).SelectedBarcode;
            var barcodeLocation = selectedBarcode.ScanLocation;

            if (barcodeLocation == null)
            {
                return;
            }

            _ = HideBarcodeInfo();
            _ = ShowFilterBar();
            await ShowMapView();


            Map.MoveToRegion(MapSpan.FromCenterAndRadius(
                new Position(barcodeLocation.Latitude.Value, barcodeLocation.Longitude.Value),
                Distance.FromKilometers(0.5)));
        }

        private async void BarcodeInfoDelete_Clicked(object sender, EventArgs e)
        {
            await HideBarcodeInfo();
        }

        private async void SearchEntry_Focused(object sender, FocusEventArgs e)
        {
            BarcodeListViewOverlay.InputTransparent = false;
            BarcodeMapViewOverlay.InputTransparent = false;
            _ = BarcodeListViewOverlay.FadeTo(OverlayMaxOpacity, 250, Easing.CubicOut);
            await BarcodeMapViewOverlay.FadeTo(OverlayMaxOpacity, 250, Easing.CubicOut);
        }

        private async void SearchEntry_Unfocused(object sender, FocusEventArgs e)
        {
            BarcodeListViewOverlay.InputTransparent = true;
            BarcodeMapViewOverlay.InputTransparent = true;
            _ = BarcodeListViewOverlay.FadeTo(0, 250, Easing.CubicOut);
            await BarcodeMapViewOverlay.FadeTo(0, 250, Easing.CubicOut);
        }
    }
}
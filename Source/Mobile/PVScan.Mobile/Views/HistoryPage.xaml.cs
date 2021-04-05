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

        // Map or List view right now?
        bool ShowingListView = false;

        HistoryPageViewModel VM;

        // Cancel event flag for when we long press a barcode
        bool CancelBarcodeTapped = false;

        public HistoryPage()
        {
            InitializeComponent();

            LayoutChanged += async (s, e) =>
            {
                _ = ShowListView(0);
                _ = HideFilterView(0);
                _ = HideBarcodeInfo(0);
                _ = HideBarcodeMapsInfo(0);
                _ = HideNoLocationPopup(0);
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

            VM.PropertyChanged += VM_PropertyChanged;
            VM.SelectedBarcodes.CollectionChanged += SelectedBarcodes_CollectionChanged;
            VM.BarcodeCopiedToClipboard += (s, e) =>
            {
                CancelBarcodeTapped = true;
            };

            //InitializeNormalBarcodeItemTemplate();

            MessagingCenter.Subscribe(this, nameof(FilterAppliedMessage),
                async (FilterPageViewModel vm, FilterAppliedMessage args) =>
                {
                    await HideFilterView();
                });
        }

        public async Task Initialize()
        {
            await VM.LoadBarcodesFromDB();

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
                    DeleteButton.FadeTo(0.5, 250, Easing.CubicOut);
                }
                else
                {
                    DeleteButton.FadeTo(1, 250, Easing.CubicOut);
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

                    DoneButton.InputTransparent = false;
                    DeleteButton.InputTransparent = false;

                    BarcodesCollectionView.SelectionMode = SelectionMode.Multiple;

                    _ = DeleteButton.FadeTo(0.5, 250, Easing.CubicOut);
                    _ = DoneButton.FadeTo(1, 250, Easing.CubicOut);
                    _ = await EditButton.FadeTo(0, 250, Easing.CubicOut);

                    //InitializeSelectableBarcodeItemTemplate();
                }
                else
                {
                    EditButton.InputTransparent = false;

                    DoneButton.InputTransparent = true;
                    DeleteButton.InputTransparent = true;

                    BarcodesCollectionView.SelectionMode = SelectionMode.None;

                    _ = DeleteButton.FadeTo(0, 250, Easing.CubicOut);
                    _ = DoneButton.FadeTo(0, 250, Easing.CubicOut);
                    _ = await EditButton.FadeTo(1, 250, Easing.CubicOut);

                    //InitializeNormalBarcodeItemTemplate();
                }
            }
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
                VM.SearchCommand.Execute(null);
            });
        }

        private void SearchEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchDelayTimer.Interval = SearchDelay;
            SearchDelayTimer.Enabled = true;
        }

        private async Task ShowListView(uint duration = 250)
        {
            ShowingListView = true;

            _ = BarcodesListContainer.TranslateTo(0, 0, duration, Easing.CubicOut);
            _ = UpperFilterBarPageIndicator.TranslateTo(0, 0, duration, Easing.CubicOut);
            await MapViewContainer.TranslateTo(MapViewContainer.Width, 0, duration, Easing.CubicOut);

            ListViewButton.Opacity = 1;
            MapViewButton.Opacity = 0.35;
        }

        private async Task ShowMapView(uint duration = 250)
        {
            ShowingListView = false;

            _ = MapViewContainer.TranslateTo(0, 0, duration, Easing.CubicOut);
            _ = UpperFilterBarPageIndicator.TranslateTo(UpperFilterBarPageIndicator.Width, 0, duration, Easing.CubicOut);
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

        private async void BarcodeListViewOverlay_Tapped(object sender, EventArgs e)
        {
            await HideBarcodeInfo();
        }

        private async Task HideBarcodeInfo(uint duration = 250)
        {
            BarcodeListViewOverlay.InputTransparent = true;

            _ = BarcodeListViewOverlay.FadeTo(0, duration, Easing.CubicOut);
            await BarcodeInfo.TranslateTo(0, BarcodeInfo.Height, duration, Easing.CubicOut);
        }

        private async Task ShowBarcodeInfo(uint duration = 250)
        {
            BarcodeListViewOverlay.InputTransparent = false;

            _ = BarcodeListViewOverlay.FadeTo(OverlayMaxOpacity, duration, Easing.CubicOut);
            _ = HideBarcodeMapsInfo(duration);
            await BarcodeInfo.TranslateTo(0, 1, duration, Easing.CubicOut);
        }

        private async Task HideBarcodeMapsInfo(uint duration = 250)
        {
            await BarcodeMapsInfo.TranslateTo(0, BarcodeMapsInfo.Height, duration, Easing.CubicOut);
        }

        private async Task ShowBarcodeMapsInfo(uint duration = 250)
        {
            _ = HideBarcodeInfo(duration);
            await BarcodeMapsInfo.TranslateTo(0, 1, duration, Easing.CubicOut);
        }

        private async void BarcodeMapsInfo_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (e.StatusType == GestureStatus.Running)
            {
                double newTranslationY = BarcodeMapsInfo.TranslationY;

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

                BarcodeMapsInfo.TranslationY = newTranslationY;
            }
            else if (e.StatusType == GestureStatus.Completed)
            {
                if (BarcodeMapsInfo.TranslationY < (BarcodeMapsInfo.Height * 0.25))
                {
                    await ShowBarcodeMapsInfo();
                }
                else
                {
                    await HideBarcodeMapsInfo();
                }
            }
        }

        private async void BarcodeInfo_PanUpdated(object sender, PanUpdatedEventArgs e)
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
                BarcodeListViewOverlay.Opacity = newOverlayOpacity;

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
            if (CancelBarcodeTapped)
            {
                CancelBarcodeTapped = false;
                return;
            }

            await ShowBarcodeInfo();
        }

        private async void BarcodeInfoShowOnMap_Clicked(object sender, EventArgs e)
        {
            Barcode selectedBarcode = VM.SelectedBarcode;
            var barcodeLocation = selectedBarcode.ScanLocation;

            if (barcodeLocation == null)
            {
                return;
            }

            await HideBarcodeInfo();
            await ShowMapView();

            Map.MoveToRegion(MapSpan.FromCenterAndRadius(
                new Position(barcodeLocation.Latitude.Value, barcodeLocation.Longitude.Value),
                Distance.FromKilometers(0.1)));
        }

        private async void BarcodeInfoDelete_Clicked(object sender, EventArgs e)
        {
            _ = HideBarcodeInfo();
            _ = HideBarcodeMapsInfo();
        }

        private async void SearchEntry_Focused(object sender, FocusEventArgs e)
        {
            BarcodeMapViewOverlay.InputTransparent = false;

            if (ShowingListView)
            {
                _ = BarcodeInfo.TranslateTo(0, BarcodeInfo.Height, 250, Easing.CubicOut);
                _ = BarcodeListViewOverlay.FadeTo(OverlayMaxOpacity, 250, Easing.CubicOut);
                BarcodeListViewOverlay.InputTransparent = false;
            }

            await BarcodeMapViewOverlay.FadeTo(OverlayMaxOpacity, 250, Easing.CubicOut);
        }

        private async void SearchEntry_Unfocused(object sender, FocusEventArgs e)
        {
            BarcodeListViewOverlay.InputTransparent = true;

            if (ShowingListView)
            {
                _ = BarcodeInfo.TranslateTo(0, BarcodeInfo.Height, 250, Easing.CubicOut);
                _ = BarcodeListViewOverlay.FadeTo(0, 250, Easing.CubicOut);
                BarcodeListViewOverlay.InputTransparent = true;
            }

            await BarcodeMapViewOverlay.FadeTo(0, 250, Easing.CubicOut);
        }

        private async void BarcodeInfoMapsShowOnList_Clicked(object sender, EventArgs e)
        {
            VM.HighlightedBarcode = null;

            var selectedBarcodeIndex = VM.Barcodes.IndexOf(VM.SelectedBarcode);

            while (VM.BarcodesPaged.Count - 1 < selectedBarcodeIndex)
            {
                // Load required barcodes until paged contains the one we selected on map
                VM.LoadNextPage.Execute(null);
            }

            await HideBarcodeMapsInfo();
            await ShowListView();

            await Task.Delay(100);

            BarcodesCollectionView.ScrollTo(selectedBarcodeIndex, -1, ScrollToPosition.Center, false);

            await Task.Delay(250);

            VM.HighlightedBarcode = VM.SelectedBarcode;

            VM.HighlightedBarcode = null;
        }

        private async void PinMarkerInfoWindow_Clicked(object sender, PinClickedEventArgs e)
        {
            var VM = (BindingContext as HistoryPageViewModel);

            VM.SelectBarcodeCommand.Execute((sender as Pin).BindingContext as Barcode);

            await ShowBarcodeMapsInfo();
        }

        private async void PinMarker_Clicked(object sender, PinClickedEventArgs e)
        {
            await HideBarcodeMapsInfo();
        }

        private async void Map_MapClicked(object sender, MapClickedEventArgs e)
        {
            await HideBarcodeMapsInfo();
        }

        private async void Barcode_NoLocationTapped(object sender, EventArgs e)
        {
            VM.NoLocationSelectedBarcode = (sender as BindableObject).BindingContext as Barcode;
            await ShowNoLocationPopup();
        }

        private async Task HideNoLocationPopup(uint duration = 250)
        {
            NoLocationPopupOverlay.InputTransparent = true;
            NoLocationPopupContainer.InputTransparent = true;
            _ = NoLocationPopupOverlay.FadeTo(0, duration, Easing.CubicOut);
            _ = NoLocationPopupContainer.FadeTo(0, duration, Easing.CubicOut);
            _ = NoLocationPopupContainer.ScaleTo(0.925, duration, Easing.CubicOut);
        }

        private async Task ShowNoLocationPopup(uint duration = 250)
        {
            NoLocationPopupOverlay.InputTransparent = false;
            NoLocationPopupContainer.InputTransparent = false;
            _ = NoLocationPopupOverlay.FadeTo(OverlayMaxOpacity / 2, duration, Easing.CubicOut);
            _ = NoLocationPopupContainer.FadeTo(1, duration, Easing.CubicOut);
            _ = NoLocationPopupContainer.ScaleTo(1, duration, Easing.CubicOut);
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await HideNoLocationPopup();
        }

        private async void SpecifyButton_Clicked(object sender, EventArgs e)
        {
            await HideNoLocationPopup();
        }

        private async void NoLocationPopupCloseButton_Clicked(object sender, EventArgs e)
        {
            await HideNoLocationPopup();
        }
    }
}
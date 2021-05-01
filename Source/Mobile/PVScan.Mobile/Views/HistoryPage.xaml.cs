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
using PVScan.Mobile.ViewModels.Messages;

namespace PVScan.Mobile.Views
{
    // IMPORTANT
    // I use 1px offsets when translating UI elements because on Anroid sometimes UI elements don't line up
    // like they do on iOS. So this little hack is fine, I guess ;)
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoryPage : ContentView
    {
        double FilterPageHeight;
        double BarcodeInfoPageHeight;
        double MapsBarcodeInfoPageHeight;

        double OverlayMaxOpacity = 0.65;

        double SearchDelay = 500;
        Timer SearchDelayTimer;

        // Map or List view right now?
        bool ShowingListView = false;

        HistoryPageViewModel VM;

        // Cancel event flag for when we long press a barcode
        bool CancelBarcodeTapped = false;

        readonly SpecifyLocationPage SpecifyLocation;

        public HistoryPage()
        {
            InitializeComponent();

            LayoutChanged += async (s, e) =>
            {
                _ = ShowListView(0);
                _ = HideFilterPage(0);
                _ = HideSortingPage(0);
                _ = HideBarcodeInfoPage(0);
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
                    await HideFilterPage();
                });

            MessagingCenter.Subscribe(this, nameof(SortingAppliedMessage),
                async (SortingPageViewModel vm, SortingAppliedMessage args) =>
                {
                    await HideSortingPage();
                });

            SpecifyLocation = new SpecifyLocationPage();
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
            else if (e.PropertyName == nameof(VM.PageCount))
            {
                Console.WriteLine(VM.PageCount);

                if (VM.PageCount == 1)
                {
                    if (VM.BarcodesPaged.Count != 0)
                    {
                        await ShowSortingFilter();
                    }
                    else
                    {
                        await HideSortingFilter();
                    }
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
                    await HideFilterPage();
                }
            }
        }

        private async void FilterButton_Clicked(object sender, EventArgs e)
        {
            await ShowFilterView();
        }

        private async void FilterPageOverlay_Tapped(object sender, EventArgs e)
        {
            await HideFilterPage();
        }

        private async Task HideFilterPage(uint duration = 350)
        {
            FilterPageOverlay.InputTransparent = true;

            _ = FilterPageOverlay.FadeTo(0, duration, Easing.CubicOut);
            await FilterPage.TranslateTo(0, FilterPageHeight, duration, Easing.CubicOut);

            (FilterPage.BindingContext as FilterPageViewModel).SetStateFromCurrentFilter();
            await FilterPage.ScrollToTop();
        }

        private async Task ShowFilterView(uint duration = 350)
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

                BarcodeInfoPageHeight = Height * 0.65;
                MapsBarcodeInfoPageHeight = Height * 0.35;

                if (BarcodeInfoPageHeight < 350)
                {
                    BarcodeInfoPageHeight = 350;
                }

                if (MapsBarcodeInfoPageHeight < 350)
                {
                    MapsBarcodeInfoPageHeight = 350;
                }

                BarcodeInfoPage.HeightRequest = BarcodeInfoPageHeight;
                BarcodeInfoPage.TranslationY = BarcodeInfoPageHeight;

                MapsBarcodeInfoPage.HeightRequest = MapsBarcodeInfoPageHeight;
                MapsBarcodeInfoPage.TranslationY = MapsBarcodeInfoPageHeight;
            }
        }

        private async void BarcodeListViewOverlay_Tapped(object sender, EventArgs e)
        {
            await HideBarcodeInfoPage();
        }

        private async Task HideBarcodeInfoPage(uint duration = 300)
        {
            BarcodeListViewOverlay.InputTransparent = true;

            _ = BarcodeListViewOverlay.FadeTo(0, duration, Easing.CubicOut);
            await BarcodeInfoPage.TranslateTo(0, BarcodeInfoPage.Height, duration, Easing.CubicOut);
            await BarcodeInfoPage.ScrollToTop();
        }

        private async Task ShowBarcodeInfoPage(uint duration = 300)
        {
            BarcodeListViewOverlay.InputTransparent = false;

            _ = BarcodeListViewOverlay.FadeTo(OverlayMaxOpacity, duration, Easing.CubicOut);
            _ = HideBarcodeMapsInfo(duration);
            await BarcodeInfoPage.TranslateTo(0, 1, duration, Easing.CubicOut);
        }

        private async void BarcodeInfoPage_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (e.StatusType == GestureStatus.Running)
            {
                double newTranslationY = BarcodeInfoPage.TranslationY;

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

                double newOverlayOpacity = OverlayMaxOpacity - ((newTranslationY / BarcodeInfoPage.Height) * OverlayMaxOpacity);

                BarcodeInfoPage.TranslationY = newTranslationY;
                BarcodeListViewOverlay.Opacity = newOverlayOpacity;

                //Console.WriteLine($"\nTOTAL_T, NEW_TRANS_Y: {e.TotalY}, {newTranslationY}");
            }
            else if (e.StatusType == GestureStatus.Completed)
            {
                if (BarcodeInfoPage.TranslationY < (BarcodeInfoPage.Height * 0.25))
                {
                    await ShowBarcodeInfoPage();
                }
                else
                {
                    await HideBarcodeInfoPage();
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

            await ShowBarcodeInfoPage();
        }

        private async void SearchEntry_Focused(object sender, FocusEventArgs e)
        {
            BarcodeMapViewOverlay.InputTransparent = false;

            if (ShowingListView)
            {
                _ = BarcodeInfoPage.TranslateTo(0, BarcodeInfoPage.Height, 250, Easing.CubicOut);
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
                _ = BarcodeInfoPage.TranslateTo(0, BarcodeInfoPage.Height, 250, Easing.CubicOut);
                _ = BarcodeListViewOverlay.FadeTo(0, 250, Easing.CubicOut);
                BarcodeListViewOverlay.InputTransparent = true;
            }

            await BarcodeMapViewOverlay.FadeTo(0, 250, Easing.CubicOut);
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
            SpecifyLocationButton.InputTransparent = true;
            NoLocationPopupOverlay.InputTransparent = true;
            NoLocationPopupContainer.InputTransparent = true;
            _ = NoLocationPopupOverlay.FadeTo(0, duration, Easing.CubicOut);
            _ = NoLocationPopupContainer.FadeTo(0, duration, Easing.CubicOut);
            _ = NoLocationPopupContainer.ScaleTo(0.925, duration, Easing.CubicOut);
        }

        private async Task ShowNoLocationPopup(uint duration = 250)
        {
            SpecifyLocationButton.InputTransparent = false;
            NoLocationPopupOverlay.InputTransparent = false;
            NoLocationPopupContainer.InputTransparent = false;
            _ = NoLocationPopupOverlay.FadeTo(OverlayMaxOpacity / 1.5, duration, Easing.CubicOut);
            _ = NoLocationPopupContainer.FadeTo(1, duration, Easing.CubicOut);
            _ = NoLocationPopupContainer.ScaleTo(1, duration, Easing.CubicOut);
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await HideNoLocationPopup();
        }

        private async void SpecifyButton_Clicked(object sender, EventArgs e)
        {
            SpecifyLocationButton.InputTransparent = true;
            SpecifyLocation.Initialize(VM.NoLocationSelectedBarcode);
            await Application.Current.MainPage.Navigation.PushModalAsync(SpecifyLocation, true);
            await HideNoLocationPopup();
        }

        private async Task HideSortingFilter(uint duration = 250)
        {
            _ = SortingAndFilterContainer.TranslateTo(0, SortingAndFilterContainer.Height, duration, Easing.CubicOut);
        }

        private async Task ShowSortingFilter(uint duration = 250)
        {
            _ = SortingAndFilterContainer.TranslateTo(0, 0, duration, Easing.CubicOut);
        }

        private async void BarcodesCollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            if (e.VerticalOffset <= 0 ||
                Math.Abs(e.VerticalDelta) < 2)
            {
                return;
            }

            if (e.VerticalDelta < 0)
            {
                _ = ShowSortingFilter(250);
            }
            else
            {
                _ = HideSortingFilter(250);
            }
        }

        private async Task HideSortingPage(uint duration = 350)
        {
            SortingPageOverlay.InputTransparent = true;

            _ = SortingPageOverlay.FadeTo(0, duration, Easing.CubicOut);
            await SortingPage.TranslateTo(0, SortingPage.Height, duration, Easing.CubicOut);

            (SortingPage.BindingContext as SortingPageViewModel).SetStateToCurrentSorting();
        }

        private async Task ShowSortingPage(uint duration = 350)
        {
            SortingPageOverlay.InputTransparent = false;

            _ = SortingPageOverlay.FadeTo(OverlayMaxOpacity, duration, Easing.CubicOut);
            await SortingPage.TranslateTo(0, 1, duration, Easing.CubicOut);
        }

        private async void SortingPage_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (e.StatusType == GestureStatus.Running)
            {
                double newTranslationY = SortingPage.TranslationY;

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

                double newOverlayOpacity = OverlayMaxOpacity - ((newTranslationY / SortingPage.Height) * OverlayMaxOpacity);

                SortingPage.TranslationY = newTranslationY;
                SortingPageOverlay.Opacity = newOverlayOpacity;

                //Console.WriteLine($"\nTOTAL_T, NEW_TRANS_Y: {e.TotalY}, {newTranslationY}");
            }
            else if (e.StatusType == GestureStatus.Completed)
            {
                if (SortingPage.TranslationY < (SortingPage.Height * 0.25))
                {
                    await ShowSortingPage();
                }
                else
                {
                    await HideSortingPage();
                }
            }
        }

        private async void SortingPageOverlay_Tapped(object sender, EventArgs e)
        {
            await HideSortingPage();
        }

        private async void SortingButton_Clicked(object sender, EventArgs e)
        {
            await ShowSortingPage();
        }

        private async void BarcodeInfoPage_Deleted(object sender, Barcode e)
        {
            await HideBarcodeInfoPage();

            VM.DeleteBarcodeCommand.Execute(VM.SelectedBarcode);
        }

        private async void BarcodeInfoPage_ShowOnMap(object sender, Barcode e)
        {
            Barcode selectedBarcode = VM.SelectedBarcode;
            var barcodeLocation = selectedBarcode.ScanLocation;

            if (barcodeLocation == null)
            {
                return;
            }

            await HideBarcodeInfoPage();
            await ShowMapView();

            Map.MoveToRegion(MapSpan.FromCenterAndRadius(
                new Position(barcodeLocation.Latitude.Value, barcodeLocation.Longitude.Value),
                Distance.FromKilometers(0.01)));
        }

        private async Task HideBarcodeMapsInfo(uint duration = 300)
        {
            await MapsBarcodeInfoPage.TranslateTo(0, MapsBarcodeInfoPage.Height, duration, Easing.CubicOut);
            await MapsBarcodeInfoPage.ScrollToTop();
        }

        private async Task ShowBarcodeMapsInfo(uint duration = 300)
        {
            _ = HideBarcodeInfoPage(duration);
            await MapsBarcodeInfoPage.TranslateTo(0, 1, duration, Easing.CubicOut);
        }

        private async void MapsBarcodeInfoPage_Deleted(object sender, Barcode e)
        {
            VM.DeleteBarcodeCommand.Execute(VM.SelectedBarcode);
            _ = HideBarcodeInfoPage();
            _ = HideBarcodeMapsInfo();
        }

        private async void MapsBarcodeInfoPage_ShowInList(object sender, Barcode e)
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

        private async void MapsBarcodeInfoPage_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (e.StatusType == GestureStatus.Running)
            {
                double newTranslationY = MapsBarcodeInfoPage.TranslationY;

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

                MapsBarcodeInfoPage.TranslationY = newTranslationY;
            }
            else if (e.StatusType == GestureStatus.Completed)
            {
                if (MapsBarcodeInfoPage.TranslationY < (MapsBarcodeInfoPage.Height * 0.25))
                {
                    await ShowBarcodeMapsInfo();
                }
                else
                {
                    await HideBarcodeMapsInfo();
                }
            }
        }

        private async void BarcodeInfoPage_NoLocationClicked(object sender, Barcode e)
        {
            VM.NoLocationSelectedBarcode = e;
            await ShowNoLocationPopup();
        }
    }
}
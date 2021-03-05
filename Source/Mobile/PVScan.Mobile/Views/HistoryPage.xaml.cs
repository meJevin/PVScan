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

namespace PVScan.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoryPage : ContentView
    {
        double FilterPageHeight;
        double OverlayMaxOpacity = 0.65;

        double SearchDelay = 500;
        Timer SearchDelayTimer;

        bool FilterBarHidden;

        public HistoryPage()
        {
            InitializeComponent();

            LayoutChanged += async (s, e) =>
            {
                _ = ShowListView(0);
                _ = HideFilterBar(0);
                _ = HideFilterView(0);
            };

            SearchDelayTimer = new Timer(SearchDelay);
            SearchDelayTimer.Enabled = true;
            SearchDelayTimer.Elapsed += SearchDelayTimer_Elapsed;
            SearchDelayTimer.AutoReset = false;

            Overlay.Opacity = 0;
            Overlay.InputTransparent = true;

            if (Device.RuntimePlatform == Device.Android)
            {
                BarcodesRefreshView.RefreshColor = Color.Black;
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                BarcodesRefreshView.SetAppThemeColor(RefreshView.RefreshColorProperty, Color.Black, Color.White);
            }

            MessagingCenter.Subscribe(this, nameof(FilterAppliedMessage),
                async (FilterPageViewModel vm, FilterAppliedMessage args) =>
                {
                    await HideFilterView();
                });
        }

        public async Task Initialize()
        {
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
                Overlay.Opacity = newOverlayOpacity;

                Console.WriteLine($"\nTOTAL_T, NEW_TRANS_Y: {e.TotalY}, {newTranslationY}");
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

        private async void Overlay_Tapped(object sender, EventArgs e)
        {
            await HideFilterView();
        }

        private async Task HideFilterView(uint duration = 250)
        {
            Overlay.InputTransparent = true;

            _ = Overlay.FadeTo(0, duration, Easing.CubicOut);
            await FilterPage.TranslateTo(0, FilterPageHeight, duration, Easing.CubicOut);
        }

        private async Task ShowFilterView(uint duration = 250)
        {
            Overlay.InputTransparent = false;

            _ = Overlay.FadeTo(OverlayMaxOpacity, duration, Easing.CubicOut);
            await FilterPage.TranslateTo(0, 1, duration, Easing.CubicOut);
        }

        private void SearchDelayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            (BindingContext as HistoryPageViewModel).SearchCommand.Execute(null);
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
    }
}
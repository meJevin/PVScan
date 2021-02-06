using PVScan.Mobile.ViewModels;
using PVScan.Mobile.ViewModels.Messages.Filtering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PVScan.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoryPage : ContentView
    {
        double FilterPageHeight;
        double OverlayMaxOpacity = 0.65;

        public HistoryPage()
        {
            InitializeComponent();

            Overlay.Opacity = 0;
            Overlay.InputTransparent = true;

            if (Device.RuntimePlatform == Device.Android)
            {
                RefreshView.RefreshColor = Color.Black;
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                RefreshView.SetAppThemeColor(RefreshView.RefreshColorProperty, Color.Black, Color.White);
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

        private async Task HideFilterView()
        {
            Overlay.InputTransparent = true;
            _ = Overlay.FadeTo(0, 250, Easing.CubicOut);
            await FilterPage.TranslateTo(0, FilterPageHeight, 250, Easing.CubicOut);
        }

        private async Task ShowFilterView()
        {
            Overlay.InputTransparent = false;
            _ = Overlay.FadeTo(OverlayMaxOpacity, 250, Easing.CubicOut);
            await FilterPage.TranslateTo(0, 1, 250, Easing.CubicOut);
        }

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
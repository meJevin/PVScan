using PVScan.Mobile.ViewModels;
using System;
using System.Collections.Generic;
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
        double FilterPageHeight = 425;
        double OverlayMaxOpacity = 0.65;

        public HistoryPage()
        {
            InitializeComponent();

            FilterPage.HeightRequest = FilterPageHeight;
            FilterPage.TranslationY = FilterPageHeight;

            Overlay.Opacity = 0;
            Overlay.InputTransparent = true;
        }

        public async Task Initialize()
        {
            await (BindingContext as HistoryPageViewModel).LoadBarcodesFromDB();
        }

        private async void FilterViewPanGesture_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            double newTranslationY = FilterPage.TranslationY;
            double newOverlayOpacity = OverlayMaxOpacity - ((newTranslationY / FilterPageHeight) * OverlayMaxOpacity);

            if (e.StatusType == GestureStatus.Running)
            {
                newTranslationY += e.TotalY;

                if (newTranslationY < 1)
                {
                    newTranslationY = 1;
                }

                FilterPage.TranslationY = newTranslationY;
                Overlay.Opacity = newOverlayOpacity;
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
    }
}
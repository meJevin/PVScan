using System;
using System.Collections.Generic;
using PVScan.Mobile.Models;
using PVScan.Mobile.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace PVScan.Mobile.Views
{
    public partial class SpecifyLocationPage : ContentPage
    {
        SpecifyLocationPageViewModel VM;

        public SpecifyLocationPage()
        {
            InitializeComponent();

            VM = BindingContext as SpecifyLocationPageViewModel;
        }

        public void Initialize(Barcode barcode)
        {
            VM.SelectedBarcode = barcode;
            VM.SelectedCoordinate.Clear();
        }

        private void Map_MapClicked(object sender, MapClickedEventArgs e)
        {
            VM.ChangeSelectedCoordianteCommand.Execute(new Coordinate()
            {
                Latitude = e.Position.Latitude,
                Longitude = e.Position.Longitude,
            });
        }

        private async void ContentPage_Appearing(object sender, EventArgs e)
        {
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

            var safeInsets = On<iOS>().SafeAreaInsets();

            DoneButton.Margin = new Thickness(0, 0, 0, safeInsets.Bottom + 12);
        }
    }
}

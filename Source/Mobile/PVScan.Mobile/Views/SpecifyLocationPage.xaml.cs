using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
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

            if (Device.RuntimePlatform == Device.Android)
            {
                BackButton.TextColor = Color.Black;
            }

            VM = BindingContext as SpecifyLocationPageViewModel;

            VM.SelectedCoordinate.CollectionChanged += SelectedCoordinate_CollectionChanged;
        }

        private void SelectedCoordinate_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (VM.SelectedCoordinate.Count == 0)
            {
                DoneButton.InputTransparent = true;
                _ = DoneButton.FadeTo(0.5, 250, Easing.CubicOut);
            }
            else
            {
                DoneButton.InputTransparent = false;
                _ = DoneButton.FadeTo(1, 250, Easing.CubicOut);
            }
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
            DoneButton.InputTransparent = false;
            BackButton.InputTransparent = false;

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

        private void DoneButton_Clicked(object sender, EventArgs e)
        {
            DoneButton.InputTransparent = true;
        }

        private void BackButton_Clicked(object sender, EventArgs e)
        {
            BackButton.InputTransparent = true;
        }
    }
}

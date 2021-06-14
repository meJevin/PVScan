using MapboxNetCore;
using PVScan.Core.Models;
using PVScan.Core.Services.Interfaces;
using PVScan.Desktop.WPF.ViewModels.Messages.Barcodes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace PVScan.Desktop.WPF.ViewModels
{
    public class MapPageViewModel : BaseViewModel
    {
        public MapPageViewModel()
        {
            CenterLocation = new GeoLocation(0, 0);
            Zoom = 13;
            Barcodes = new ObservableCollection<Barcode>();

            MessagingCenter.Subscribe<HistoryPageViewModel, HistoryPageBarcodesCollectionChanged>(this,
                nameof(HistoryPageBarcodesCollectionChanged), (sender, args) => {
                    if (args.Args.Action == NotifyCollectionChangedAction.Add)
                    {
                        foreach (Barcode b in args.Args.NewItems)
                        {
                            Barcodes.Add(b);
                        }
                    }
                    else if (args.Args.Action == NotifyCollectionChangedAction.Remove)
                    {
                        foreach (Barcode b in args.Args.OldItems)
                        {
                            Barcodes.Remove(b);
                        }
                    }
                    else if (args.Args.Action == NotifyCollectionChangedAction.Reset)
                    {
                        Barcodes.Clear();
                    }
                });
        }

        public async Task Initialize()
        {
            // TOdo: get this into a service!
            var geolocationStatus = await Geolocator.RequestAccessAsync();

            if (geolocationStatus == GeolocationAccessStatus.Allowed)
            {
                Geolocator geolocator = new Geolocator()
                {
                    DesiredAccuracy = PositionAccuracy.High,
                };
                var location = await geolocator.GetGeopositionAsync();

                CenterLocation = new GeoLocation(location.Coordinate.Latitude, location.Coordinate.Longitude);
            }
        }

        public ObservableCollection<Barcode> Barcodes { get; set; }

        public GeoLocation CenterLocation { get; set; }
        public double Zoom { get; set; }
    }
}

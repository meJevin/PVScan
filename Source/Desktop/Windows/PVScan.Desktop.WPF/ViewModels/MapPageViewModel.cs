using MapboxNetCore;
using PVScan.Core.Models;
using PVScan.Core.Models.API;
using PVScan.Core.Services.Interfaces;
using PVScan.Desktop.WPF.ViewModels.Messages.Barcodes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Devices.Geolocation;

namespace PVScan.Desktop.WPF.ViewModels
{
    public class MapPageViewModel : BaseViewModel
    {
        readonly IBarcodesRepository BarcodesRepository;
        readonly IPVScanAPI PVScanAPI;
        readonly IAPIBarcodeHub BarcodeHub;

        public MapPageViewModel(IBarcodesRepository barcodesRepository,
            IPVScanAPI pVScanAPI,
            IAPIBarcodeHub barcodeHub)
        {
            BarcodesRepository = barcodesRepository;
            PVScanAPI = pVScanAPI;
            BarcodeHub = barcodeHub;

            CenterLocation = new GeoLocation(0, 0);
            Zoom = 13;
            Barcodes = new ObservableCollection<Barcode>();

            LocationSpecifiedCommand = new Command(async () =>
            {
                var newCoord = new Coordinate()
                {
                    Latitude = CenterLocation.Latitude,
                    Longitude = CenterLocation.Longitude,
                };

                LocationSpecificationBarcode.ScanLocation = newCoord;
                await BarcodesRepository.Update(LocationSpecificationBarcode);

                MessagingCenter.Send(this, nameof(LocationSpecifiedMessage),
                    new LocationSpecifiedMessage()
                    {
                        Barcode = LocationSpecificationBarcode,
                    });

                var req = new UpdatedBarcodeRequest()
                {
                    GUID = LocationSpecificationBarcode.GUID,
                    Latitude = LocationSpecificationBarcode.ScanLocation?.Latitude,
                    Longitude = LocationSpecificationBarcode.ScanLocation?.Longitude,
                    Favorite = LocationSpecificationBarcode.Favorite,
                };
                await PVScanAPI.UpdatedBarcode(req);

                await BarcodeHub.Updated(req);

                IsSpecifyingLocation = false;
                LocationSpecificationBarcode = null;
            });

            CancelLocationSpecificationCommand = new Command(() =>
            {
                IsSpecifyingLocation = false;
                LocationSpecificationBarcode = null;
            });

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
            var geolocationStatus = await Geolocator.RequestAccessAsync().AsTask();

            if (geolocationStatus == GeolocationAccessStatus.Allowed)
            {
                Geolocator geolocator = new Geolocator()
                {
                    DesiredAccuracy = PositionAccuracy.High,
                };
                var location = await geolocator.GetGeopositionAsync().AsTask();

                CenterLocation = new GeoLocation(location.Coordinate.Latitude, location.Coordinate.Longitude);
            }
        }

        public ObservableCollection<Barcode> Barcodes { get; set; }

        public ICommand LocationSpecifiedCommand { get; set; }
        public ICommand CancelLocationSpecificationCommand { get; set; }
        
        public bool IsSpecifyingLocation { get; set; }
        public Barcode LocationSpecificationBarcode { get; set; }
        public GeoLocation CenterLocation { get; set; }
        public double Zoom { get; set; }
    }
}

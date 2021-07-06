using MapboxNetCore;
using PVScan.Core.Models;
using PVScan.Desktop.WPF.ViewModels;
using PVScan.Desktop.WPF.ViewModels.Messages.Barcodes;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PVScan.Desktop.WPF.Views
{
    /// <summary>
    /// Interaction logic for MapPage.xaml
    /// </summary>
    public partial class MapPage : ContentControl
    {
        Dictionary<string, Barcode> BarcodeMarkers = new Dictionary<string, Barcode>();

        MapPageViewModel VM;

        public event EventHandler<Barcode> BarcodeSelected;

        public MapPage()
        {
            InitializeComponent();
            Map.AccessToken = Constants.MapBoxKey;
            Map.Ready += Map_Ready;

            VM = DataContext as MapPageViewModel;

            MessagingCenter.Subscribe(this, nameof(ShowBarcodeOnMapMessage),
                async (MainWindowViewModel vm, ShowBarcodeOnMapMessage args) =>
                {
                    if (args.BarcodeToShow.ScanLocation == null)
                    {
                        return;
                    }

                    var loc = args.BarcodeToShow.ScanLocation;

                    Map.FlyTo(new GeoLocation(loc.Longitude.Value, loc.Latitude.Value), 12);
                });
        }

        private void Barcodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Barcode b in e.NewItems)
                {
                    AddMarker(b);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Barcode b in e.OldItems)
                {
                    RemoveMarker(b.GUID);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                ClearMarkers();
            }
        }

        private void AddMarker(Barcode b)
        {
            if (b.ScanLocation == null)
            {
                return;
            }

            Map.AddMarker(new GeoLocation(b.ScanLocation.Latitude.Value, b.ScanLocation.Longitude.Value), b.GUID);
            BarcodeMarkers.Add(b.GUID, b);
        }

        private void RemoveMarker(string guid)
        {
            Map.RemoveMarker(guid);
            BarcodeMarkers.Remove(guid);
        }

        private void ClearMarkers()
        {
            Map.ClearMarkers();
            BarcodeMarkers.Clear();
        }

        private async void Map_Ready(object sender, EventArgs e)
        {
            Map.Invoke.SetStyle("mapbox://styles/mapbox/dark-v10");

            await VM.Initialize();

            VM.Barcodes.CollectionChanged += Barcodes_CollectionChanged;
            foreach (Barcode b in VM.Barcodes)
            {
                AddMarker(b);
            }
        }

        private void Map_MarkerClicked(object sender, string e)
        {
            BarcodeSelected?.Invoke(this, BarcodeMarkers[e]);
        }
    }
}

using MapboxNetCore;
using PVScan.Core.Models;
using PVScan.Desktop.WPF.ViewModels;
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
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;

namespace PVScan.Desktop.WPF.Views
{
    /// <summary>
    /// Interaction logic for MapPage.xaml
    /// </summary>
    public partial class MapPage : ContentControl
    {
        Dictionary<string, Barcode> BarcodeMarkers = new Dictionary<string, Barcode>();

        MapPageViewModel VM;

        public MapPage()
        {
            InitializeComponent();
            Map.AccessToken = App.MapBoxToken;
            Map.Ready += Map_Ready;

            VM = DataContext as MapPageViewModel;
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
        }

        private void RemoveMarker(string guid)
        {
            Map.RemoveMarker(guid);
        }

        private void ClearMarkers()
        {
            Map.ClearMarkers();
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
    }
}

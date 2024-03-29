﻿using MapboxNetCore;
using PVScan.Core;
using PVScan.Core.Models;
using PVScan.Desktop.WPF.ViewModels;
using PVScan.Desktop.WPF.ViewModels.Messages.Barcodes;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
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
        MapPageViewModel VM;

        public event EventHandler<Barcode> BarcodeSelected;

        public MapPage()
        {
            InitializeComponent();
            Map.AccessToken = Constants.MapBoxKey;
            Map.Ready += Map_Ready;

            VM = DataContext as MapPageViewModel;

            VM.PropertyChanged += VM_PropertyChanged;

            MessagingCenter.Subscribe(this, nameof(ShowBarcodeOnMapMessage),
                async (MainWindowViewModel vm, ShowBarcodeOnMapMessage args) =>
                {
                    if (args.BarcodeToShow.ScanLocation == null)
                    {
                        return;
                    }

                    var loc = args.BarcodeToShow.ScanLocation;

                    Map.FlyTo(new GeoLocation(loc.Latitude.Value, loc.Longitude.Value), 12);
                });

            // When we localy specify a location
            MessagingCenter.Subscribe(this, nameof(LocationSpecifiedMessage),
                async (MapPageViewModel vm, LocationSpecifiedMessage args) =>
                {
                    await AddPoint(args.Barcode);
                });

            // When location is specified for a barcode remotely
            MessagingCenter.Subscribe(this, nameof(LocationSpecifiedMessage),
                async (HistoryPageViewModel vm, LocationSpecifiedMessage args) =>
                {
                    await AddPoint(args.Barcode);
                });

            LocationSpecificationContainer.IsHitTestVisible = false;
            _ = LocationSpecificationContainer.FadeTo(0, Animations.DefaultDuration);
        }

        private async void VM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VM.IsSpecifyingLocation))
            {
                if (VM.IsSpecifyingLocation)
                {
                    LocationSpecificationContainer.IsHitTestVisible = true;
                    _ = LocationSpecificationContainer.FadeTo(1, Animations.DefaultDuration);
                }
                else
                {
                    LocationSpecificationContainer.IsHitTestVisible = false;
                    _ = LocationSpecificationContainer.FadeTo(0, Animations.DefaultDuration);
                }
            }
        }

        private async void Barcodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Barcode b in e.NewItems)
                {
                    await AddPoint(b);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Barcode b in e.OldItems)
                {
                    await RemoveMarker(b.GUID);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                await ClearMarkers();
            }
        }

        private async Task AddPoints(IEnumerable<Barcode> barcodes)
        {
            List<MapboxPoint> toAdd = new List<MapboxPoint>();

            foreach (var b in barcodes)
            {
                if (b.ScanLocation == null)
                {
                    continue;
                }

                toAdd.Add(new MapboxPoint()
                {
                    GUID = b.GUID,
                    Latitude = b.ScanLocation.Latitude.Value,
                    Longitude = b.ScanLocation.Longitude.Value,
                    Properties =
                    new
                    {
                    },
                });
            }

            await Map.AddPoints(toAdd);
        }

        private async Task AddPoint(Barcode b)
        {
            if (b.ScanLocation == null)
            {
                return;
            }


            await Map.AddPoint(new MapboxPoint()
            {
                GUID = b.GUID,
                Latitude = b.ScanLocation.Latitude.Value,
                Longitude = b.ScanLocation.Longitude.Value,
                Properties =
                new
                {
                },
            });
        }

        private async Task RemoveMarker(string guid)
        {
            await Map.RemovePoint(guid);
        }

        private async Task ClearMarkers()
        {
            await Map.ClearPoints();
        }

        private async void Map_Ready(object sender, EventArgs e)
        {
            await VM.Initialize();

            VM.Barcodes.CollectionChanged += Barcodes_CollectionChanged;
            await AddPoints(VM.Barcodes);
        }

        private void Map_PointClicked(object sender, string e)
        {
            BarcodeSelected?.Invoke(this, VM.Barcodes.FirstOrDefault(b => b.GUID == e));
        }

        private void Map_MouseDown(object sender, EventArgs e)
        {
            _ = CenterLocationIcon.TranslateTo(0, -100, Animations.DefaultDuration);
            _ = CenterLocationEllipse.FadeTo(0.85, Animations.DefaultDuration);
        }

        private void Map_MouseUp(object sender, EventArgs e)
        {
            _ = CenterLocationIcon.TranslateTo(0, 0, Animations.DefaultDuration);
            _ = CenterLocationEllipse.FadeTo(0, Animations.DefaultDuration);
        }
    }
}

using PVScan.Core.Models;
using PVScan.Core.Models.API;
using PVScan.Core.Services.Interfaces;
using PVScan.Desktop.WPF.ViewModels.Messages.Barcodes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PVScan.Desktop.WPF.ViewModels
{
    public class BarcodeInfoPageViewModel : BaseViewModel
    {
        readonly IAPIBarcodeHub BarcodeHub;
        readonly IBarcodesRepository BarcodesRepository;

        public BarcodeInfoPageViewModel(IAPIBarcodeHub barcodeHub, 
            IBarcodesRepository barcodesRepository)
        {
            BarcodeHub = barcodeHub;
            BarcodesRepository = barcodesRepository;

            this.PropertyChanged += BarcodeInfoPageViewModel_PropertyChanged;

            MessagingCenter.Subscribe(this, nameof(LocationSpecifiedMessage),
                async (MapPageViewModel vm, LocationSpecifiedMessage args) =>
                {
                    if (SelectedBarcode == args.Barcode)
                    {
                        SelectedBarcode = null;
                        SelectedBarcode = args.Barcode;
                    }
                });

            CloseCommand = new Command(() =>
            {
                Closed?.Invoke(this, new EventArgs());
            });

            BarcodeHub.OnUpdated += BarcodeHub_OnUpdated;
            BarcodeHub.OnUpdatedMultple += BarcodeHub_OnUpdatedMultple;
            
            BarcodeHub.OnDeleted += BarcodeHub_OnDeleted;
            BarcodeHub.OnDeletedMultiple += BarcodeHub_OnDeletedMultiple;
        }
        
        private async void BarcodeHub_OnUpdated(object sender, UpdatedBarcodeRequest req)
        {
            if (SelectedBarcode == null ||
                req.GUID != SelectedBarcode.GUID)
            {
                return;
            }

            var localBarcode = await BarcodesRepository.FindByGUID(req.GUID);

            if (localBarcode == null)
            {
                return;
            }

            if (req.Longitude.HasValue && req.Latitude.HasValue)
            {
                localBarcode.ScanLocation = new Coordinate()
                {
                    Latitude = req.Latitude,
                    Longitude = req.Longitude,
                };
            }
            localBarcode.Favorite = req.Favorite;

            SelectedBarcode = null;
            SelectedBarcode = localBarcode;
        }
        private async void BarcodeHub_OnUpdatedMultple(object sender, List<UpdatedBarcodeRequest> e)
        {
            if (SelectedBarcode == null) return;

            var found = e.FirstOrDefault(d => d.GUID == SelectedBarcode.GUID);

            if (found == null)
            {
                return;
            }

            var localBarcode = await BarcodesRepository.FindByGUID(found.GUID);

            if (localBarcode == null)
            {
                return;
            }

            if (found.Longitude.HasValue && found.Latitude.HasValue)
            {
                localBarcode.ScanLocation = new Coordinate()
                {
                    Latitude = found.Latitude,
                    Longitude = found.Longitude,
                };
            }
            localBarcode.Favorite = found.Favorite;

            SelectedBarcode = null;
            SelectedBarcode = localBarcode;
        }

        private async void BarcodeHub_OnDeletedMultiple(object sender, List<DeletedBarcodeRequest> e)
        {
            if (SelectedBarcode == null) return;

            var found = e.FirstOrDefault(d => d.GUID == SelectedBarcode.GUID);

            if (SelectedBarcode == null ||
                found == null)
            {
                return;
            }

            var localBarcode = await BarcodesRepository.FindByGUID(found.GUID);

            if (localBarcode == null)
            {
                return;
            }

            SelectedBarcode = null;

            Closed?.Invoke(this, new EventArgs());
        }

        private async void BarcodeHub_OnDeleted(object sender, DeletedBarcodeRequest e)
        {
            if (SelectedBarcode == null ||
                e.GUID != SelectedBarcode.GUID)
            {
                return;
            }

            var localBarcode = await BarcodesRepository.FindByGUID(e.GUID);

            if (localBarcode == null)
            {
                return;
            }

            SelectedBarcode = null;

            Closed?.Invoke(this, new EventArgs());
        }

        private void BarcodeInfoPageViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedBarcode))
            {
                SwitchOnMapButton();
            }
        }

        private void SwitchOnMapButton()
        {
            if (SelectedBarcode == null ||
                SelectedBarcode.ScanLocation == null ||
                (!SelectedBarcode.ScanLocation.Latitude.HasValue && !SelectedBarcode.ScanLocation.Longitude.HasValue))
            {
                SelectedBarcodeHasLocation = false;
                return;
            }

            SelectedBarcodeHasLocation = true;
        }

        public Barcode SelectedBarcode { get; set; }
        public bool SelectedBarcodeHasLocation { get; set; } = true;

        public event EventHandler Closed;
        public ICommand CloseCommand { get; set; }
    }
}

using PVScan.Core.Models;
using PVScan.Core.Models.API;
using PVScan.Core.Services.Interfaces;
using PVScan.Desktop.WPF.Services.Interfaces;
using PVScan.Desktop.WPF.ViewModels.Messages.Barcodes;
using PVScan.Desktop.WPF.Views.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PVScan.Desktop.WPF.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        readonly IBarcodesRepository BarcodesRepository;
        readonly IPopup<NoLocationAvailablePopupArgs, NoLocationAvailablePopupResult> NoLocationPopup;
        readonly IPVScanAPI PVScanAPI;
        readonly IAPIBarcodeHub BarcodeHub;

        public MainWindowViewModel(IBarcodesRepository barcodesRepository,
            IPopup<NoLocationAvailablePopupArgs, NoLocationAvailablePopupResult> noLocationPopup,
            IPVScanAPI pVScanAPI,
            IAPIBarcodeHub barcodeHub)
        {
            BarcodesRepository = barcodesRepository;
            NoLocationPopup = noLocationPopup;
            PVScanAPI = pVScanAPI;
            BarcodeHub = barcodeHub;

            ToggleMapScanPages = new Command(() =>
            {
                MapScanPagesToggled?.Invoke(this, new EventArgs());
            });

            ShowInListCommand = new Command((object b) =>
            {
                // Show in list
                MessagingCenter.Send(this, nameof(ShowBarcodeInListMessage), new ShowBarcodeInListMessage()
                {
                    BarcodeToShow = b as Barcode,
                });
            });
            ShowOnMapCommand = new Command((object b) =>
            {
                // Show on map
                MessagingCenter.Send(this, nameof(ShowBarcodeOnMapMessage), new ShowBarcodeOnMapMessage()
                {
                    BarcodeToShow = b as Barcode,
                });
            });
            DeleteCommand = new Command(async (object b) =>
            {
                // Delete
                var barcode = b as Barcode;
                await BarcodesRepository.Delete(barcode);

                MessagingCenter.Send(this, nameof(BarcodeDeletedMessage), new BarcodeDeletedMessage() 
                {
                    DeletedBarcode = barcode,
                });

                var req = new DeletedBarcodeRequest()
                {
                    GUID = barcode.GUID,
                };

                if (await PVScanAPI.DeletedBarcode(req) != null)
                {
                    await BarcodeHub.Deleted(req);
                }
            });

            NoLocationCommand = new Command(async (object b) =>
            {
                var barcode = b as Barcode;

                var result = await NoLocationPopup.ShowPopup();

                if (result.UserWantsToSpecify)
                {
                    // Send message that user is specifiying location for this barcode
                    LocationSpecificationStarted?.Invoke(this, barcode);
                }

                // Do nothing..
            });
        }

        public ICommand ToggleMapScanPages { get; set; }

        public event EventHandler MapScanPagesToggled;

        public ICommand ShowOnMapCommand { get; set; }
        public ICommand ShowInListCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public ICommand NoLocationCommand { get; set; }

        public event EventHandler<Barcode> LocationSpecificationStarted;
    }
}

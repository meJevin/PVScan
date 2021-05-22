﻿using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MvvmHelpers;
using PVScan.Mobile.Models;
using PVScan.Mobile.Models.API;
using PVScan.Mobile.Services.Interfaces;
using PVScan.Mobile.ViewModels.Messages;
using Xamarin.Forms;

namespace PVScan.Mobile.ViewModels
{
    public class SpecifyLocationPageViewModel : BaseViewModel
    {
        readonly IBarcodesRepository BarcodesRepository;
        readonly IPVScanAPI PVScanAPI;
        readonly IAPIBarcodeHub BarcodeHub;

        public SpecifyLocationPageViewModel(
            IBarcodesRepository barcodesRepository,
            IPVScanAPI pVScanAPI,
            IAPIBarcodeHub barcodeHub)
        {
            BarcodesRepository = barcodesRepository;
            PVScanAPI = pVScanAPI;
            BarcodeHub = barcodeHub;

            SelectedCoordinate = new ObservableCollection<Coordinate>();

            ChangeSelectedCoordianteCommand = new Command(async (object newCoordinate) =>
            {
                var newCoord = newCoordinate as Coordinate;

                SelectedCoordinate.Clear();
                SelectedCoordinate.Add(newCoord);
            });

            DoneCommand = new Command(async () =>
            {
                if (SelectedCoordinate.Count == 0)
                {
                    await Application.Current.MainPage.Navigation.PopModalAsync(true);
                    return;
                }

                var newCoord = SelectedCoordinate[0];

                SelectedBarcode.ScanLocation = newCoord;
                await BarcodesRepository.Update(SelectedBarcode);

                MessagingCenter.Send(this, nameof(BarcodeLocationSpecifiedMessage),
                    new BarcodeLocationSpecifiedMessage()
                    {
                        UpdatedBarcode = SelectedBarcode,
                    });

                await Application.Current.MainPage.Navigation.PopModalAsync(true);

                var req = new UpdatedBarcodeRequest()
                {
                    GUID = SelectedBarcode.GUID,
                    Latitude = SelectedBarcode.ScanLocation?.Latitude,
                    Longitude = SelectedBarcode.ScanLocation?.Longitude,
                    Favorite = SelectedBarcode.Favorite,
                };
                await PVScanAPI.UpdatedBarcode(req);

                await BarcodeHub.Updated(req);
            });
        }

        public Barcode SelectedBarcode { get; set; }

        public ObservableCollection<Coordinate> SelectedCoordinate { get; set; }

        public ICommand ChangeSelectedCoordianteCommand { get; }

        public ICommand DoneCommand { get; }
    }
}

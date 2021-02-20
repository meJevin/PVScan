using Microsoft.EntityFrameworkCore;
using MvvmHelpers;
using PVScan.Mobile.DAL;
using PVScan.Mobile.Models;
using PVScan.Mobile.Services;
using PVScan.Mobile.Services.Interfaces;
using PVScan.Mobile.ViewModels.Messages.Filtering;
using PVScan.Mobile.ViewModels.Messages.Scanning;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing;

namespace PVScan.Mobile.ViewModels
{
    public class Filter
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public LastTimeType? LastType { get; set; } 

        public IEnumerable<BarcodeFormat> BarcodeFormats { get; set; }
    }

    public class HistoryPageViewModel : BaseViewModel
    {
        readonly IBarcodesRepository BarcodesRepository;

        public HistoryPageViewModel(IBarcodesRepository barcodesRepository)
        {
            BarcodesRepository = barcodesRepository;

            Barcodes = new ObservableRangeCollection<Barcode>();

            //MessagingCenter.Subscribe(this, nameof(BarcodeScannedMessage),
            //    async (ScanPageViewModel vm, BarcodeScannedMessage args) => 
            //    {
            //        Barcodes.Add(args.ScannedBarcode);
            //    });

            MessagingCenter.Subscribe(this, nameof(FilterAppliedMessage),
                async (FilterPageViewModel vm, FilterAppliedMessage args) =>
                {
                    CurrentFilter = args.NewFilter;

                    await LoadBarcodesFromDB();
                });

            RefreshCommand = new Command(async () =>
            {
                IsRefresing = true;

                await LoadBarcodesFromDB();

                IsRefresing = false;
            });

            SearchCommand = new Command(async () =>
            {
                await LoadBarcodesFromDB();
            });
        }

        public async Task LoadBarcodesFromDB()
        {
            if (IsLoading)
            {
                return;
            }

            Barcodes.Clear();

            IsLoading = true;

            IEnumerable<Barcode> dbBarcodes = null;

            if (CurrentFilter == null)
            {
                dbBarcodes = await BarcodesRepository.GetAll();
            }
            else
            {
                dbBarcodes = await BarcodesRepository.GetAllFiltered(CurrentFilter);
            }

            if (!String.IsNullOrEmpty(Search))
            {
                dbBarcodes = dbBarcodes.Where(b => b.Text.Contains(Search));
            }

            Barcodes.AddRange(dbBarcodes.OrderByDescending(b => b.ScanTime));

            IsLoading = false;
        }

        public string Search { get; set; }

        public Filter CurrentFilter { get; set; }

        public ObservableRangeCollection<Barcode> Barcodes { get; set; }

        public bool IsLoading { get; set; }

        public bool IsRefresing { get; set; }

        public ICommand RefreshCommand { get; set; }

        public ICommand SearchCommand { get; set; }
    }
}

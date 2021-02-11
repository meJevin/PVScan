using Microsoft.EntityFrameworkCore;
using MvvmHelpers;
using PVScan.Mobile.DAL;
using PVScan.Mobile.Models;
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
        readonly PVScanMobileDbContext _context;

        public HistoryPageViewModel()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "PVScan.db3");
            _context = new PVScanMobileDbContext(dbPath);

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
        }

        public async Task LoadBarcodesFromDB()
        {
            if (IsLoading)
            {
                return;
            }

            Barcodes.Clear();

            IsLoading = true;

            var dbBarcodes = _context.Barcodes.AsQueryable();

            if (CurrentFilter != null)
            {
                if (CurrentFilter.FromDate != null && CurrentFilter.ToDate != null)
                {
                    dbBarcodes = dbBarcodes
                        .Where(b => b.ScanTime >= CurrentFilter.FromDate)
                        .Where(b => b.ScanTime < CurrentFilter.ToDate);
                }
                else if (CurrentFilter.LastType != null)
                {
                    DateTime to = DateTime.Today.AddDays(1);
                    DateTime from = DateTime.Today;

                    if (CurrentFilter.LastType == LastTimeType.Day)
                    {
                        from = from.AddDays(-1);
                    }
                    else if (CurrentFilter.LastType == LastTimeType.Week)
                    {
                        from = from.AddDays(-7);
                    }
                    else if (CurrentFilter.LastType == LastTimeType.Month)
                    {
                        from = from.AddMonths(-1);
                    }
                    else if (CurrentFilter.LastType == LastTimeType.Year)
                    {
                        from = from.AddYears(-1);
                    }

                    dbBarcodes = dbBarcodes
                        .Where(b => b.ScanTime >= from)
                        .Where(b => b.ScanTime < to);
                }

                if (CurrentFilter.BarcodeFormats.Any())
                {
                    dbBarcodes = dbBarcodes
                        .Where(b => CurrentFilter.BarcodeFormats.Contains(b.Format));
                }
            }

            dbBarcodes = dbBarcodes.OrderByDescending(b => b.ScanTime);

            Barcodes.AddRange(dbBarcodes);

            IsLoading = false;
        }

        private Filter CurrentFilter { get; set; }

        public ObservableRangeCollection<Barcode> Barcodes { get; set; }

        public bool IsLoading { get; set; }

        public bool IsRefresing { get; set; }

        public ICommand RefreshCommand { get; set; }
    }
}

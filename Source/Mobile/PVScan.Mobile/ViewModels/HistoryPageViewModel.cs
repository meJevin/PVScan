using Microsoft.EntityFrameworkCore;
using MvvmHelpers;
using PVScan.Mobile.DAL;
using PVScan.Mobile.Models;
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

namespace PVScan.Mobile.ViewModels
{
    public class HistoryPageViewModel : BaseViewModel
    {
        readonly PVScanMobileDbContext _context;

        public HistoryPageViewModel()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "PVScan.db3");

            Barcodes = new ObservableRangeCollection<Barcode>();
            _context = new PVScanMobileDbContext(dbPath);

            MessagingCenter.Subscribe(this, nameof(BarcodeScannedMessage),
                async (ScanPageViewModel vm, BarcodeScannedMessage args) => 
                {
                    Barcodes.Add(args.ScannedBarcode);
                });

            RefreshCommand = new Command(async () =>
            {
                IsRefresing = true;
                OnPropertyChanged(nameof(IsRefresing));

                await LoadBarcodesFromDB();

                IsRefresing = false;
                OnPropertyChanged(nameof(IsRefresing));
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
            OnPropertyChanged(nameof(IsLoading));

            await Task.Delay(2500);

            var dbBarcodes = await _context.Barcodes.OrderByDescending(b => b.ScanTime).ToListAsync();
            Barcodes.AddRange(dbBarcodes);

            IsLoading = false;
            OnPropertyChanged(nameof(IsLoading));
        }

        public ObservableRangeCollection<Barcode> Barcodes { get; set; }

        public bool IsLoading { get; set; }

        public bool IsRefresing { get; set; }
        public ICommand RefreshCommand { get; set; }
    }
}

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
        }

        public async Task Initialize()
        {
            if (Barcodes.Count != 0)
            {
                return;
            }

            var d1 = DateTime.Now;
            var dbBarcodes = await _context.Barcodes.ToListAsync();
            var d2 = DateTime.Now;

            Console.WriteLine("TOOK:" + (d2 - d1).TotalMilliseconds);

            if (dbBarcodes.Count != 0)
            {
                Barcodes.AddRange(dbBarcodes);
            }
        }

        public ObservableRangeCollection<Barcode> Barcodes { get; set; }
    }
}

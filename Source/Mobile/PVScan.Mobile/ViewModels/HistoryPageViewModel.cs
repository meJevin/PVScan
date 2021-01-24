using Microsoft.EntityFrameworkCore;
using MvvmHelpers;
using PVScan.Mobile.DAL;
using PVScan.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PVScan.Mobile.ViewModels
{
    public class HistoryPageViewModel : BaseViewModel
    {
        readonly PVScanMobileDbContext _context;

        public HistoryPageViewModel()
        {
            Barcodes = new ObservableRangeCollection<Barcode>();
            _context = new PVScanMobileDbContext();

            AddBarcodeCommand = new Command(async () =>
            {
                var barcode = new Barcode()
                {
                    Text = "Test",
                    Format = ZXing.BarcodeFormat.QR_CODE,
                    ServerSynced = false,
                    ScanLocation = new Coordinate()
                    {
                        Latitude = 30,
                        Longitude = 50,
                    },
                };

                await _context.Barcodes.AddAsync(barcode);
                await _context.SaveChangesAsync();

                Barcodes.Add(barcode);
            });
        }

        public async Task Initialize()
        {
            if (Barcodes.Count != await _context.Barcodes.CountAsync())
            {
                Barcodes.AddRange(await _context.Barcodes.ToListAsync());
            }
        }

        public ICommand AddBarcodeCommand { get; }

        public ObservableRangeCollection<Barcode> Barcodes { get; set; }
    }
}

﻿using MvvmHelpers;
using PVScan.Mobile.DAL;
using PVScan.Mobile.ViewModels.Messages.Filtering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing;

namespace PVScan.Mobile.ViewModels
{
    public class FilterPageViewModel : BaseViewModel
    {
        readonly PVScanMobileDbContext _context;

        public FilterPageViewModel()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "PVScan.db3");
            _context = new PVScanMobileDbContext(dbPath);

            // Init barcode formats
            AvailableBarcodeFormats = new ObservableRangeCollection<BarcodeFormat>();
            AvailableBarcodeFormats.AddRange(Enum.GetValues(typeof(BarcodeFormat)).OfType<BarcodeFormat>());

            SelectedBarcodeFormats = new ObservableRangeCollection<object>();

            // Init min max dates from current DB
            var minDate = _context.Barcodes.Min(b => b.ScanTime);
            var maxDate = _context.Barcodes.Max(b => b.ScanTime);

            FromDate = minDate;
            ToDate = maxDate;

            ApplyFilterCommand = new Command(() => 
            {
                MessagingCenter.Send(this, nameof(FilterAppliedMessage), new FilterAppliedMessage()
                {
                    NewFilter = new Filter()
                    {
                        BarcodeFormats = SelectedBarcodeFormats.Select(o => (BarcodeFormat)o),
                        FromDate = FromDate,
                        ToDate = ToDate,
                    }
                });
            });
        }

        public ICommand ApplyFilterCommand { get; }

        public ObservableRangeCollection<BarcodeFormat> AvailableBarcodeFormats { get; set; }
        public ObservableRangeCollection<object> SelectedBarcodeFormats { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}

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
    // Because selection in collection view on iOS doesn't like value types, we need to wrap BarcodeFormat enum in a class
    public class ZxingBarcodeFormat
    {
        public BarcodeFormat Format { get; set; }
    }

    public class FilterPageViewModel : BaseViewModel
    {
        readonly PVScanMobileDbContext _context;

        public FilterPageViewModel()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "PVScan.db3");
            _context = new PVScanMobileDbContext(dbPath);

            // Init barcode formats
            AvailableBarcodeFormats = new ObservableRangeCollection<ZxingBarcodeFormat>();
            AvailableBarcodeFormats
                .AddRange(Enum.GetValues(typeof(BarcodeFormat))
                    .OfType<BarcodeFormat>()
                    .Select(v => new ZxingBarcodeFormat() { Format = v }));

            SelectedBarcodeFormats = new ObservableRangeCollection<object>();

            ResetFilter();

            ApplyFilterCommand = new Command(() => 
            {
                MessagingCenter.Send(this, nameof(FilterAppliedMessage), new FilterAppliedMessage()
                {
                    NewFilter = new Filter()
                    {
                        BarcodeFormats = SelectedBarcodeFormats.Select(o => ((ZxingBarcodeFormat)o).Format),
                        FromDate = FromDate,
                        ToDate = ToDate,
                    }
                });
            });
        }

        public void ResetFilter()
        {
            // Init min max dates from current DB
            DateTime minDate = DateTime.MinValue;
            DateTime maxDate = DateTime.MaxValue;

            try
            {
                minDate = _context.Barcodes.Min(b => b.ScanTime);
                maxDate = _context.Barcodes.Max(b => b.ScanTime);
            }
            catch (Exception ex)
            {
                // If no barcodes
            }

            FromDate = minDate;
            ToDate = maxDate;

            SelectedBarcodeFormats.Clear();
        }

        public ICommand ApplyFilterCommand { get; }

        public ObservableRangeCollection<ZxingBarcodeFormat> AvailableBarcodeFormats { get; set; }
        public ObservableRangeCollection<object> SelectedBarcodeFormats { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
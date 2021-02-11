using MvvmHelpers;
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
    public class ZXingBarcodeFormat
    {
        public BarcodeFormat Format { get; set; }
    }

    // Read above
    public class LastTimeSpan
    {
        public LastTimeType Type { get; set; }
    }

    // Enum for filter page to filter by last day/week/month/year
    public enum LastTimeType
    {
        Day,
        Week,
        Month,
        Year,
    }

    public class FilterPageViewModel : BaseViewModel
    {
        readonly PVScanMobileDbContext _context;

        public FilterPageViewModel()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "PVScan.db3");
            _context = new PVScanMobileDbContext(dbPath);

            InitBarcodeFormats();

            InitLastTimeSpans();

            ResetFilter();

            ApplyFilterCommand = new Command(() =>
            {
                var newFilter = new Filter()
                {
                    BarcodeFormats = SelectedBarcodeFormats.Select(o => ((ZXingBarcodeFormat)o).Format),
                };

                if (DateFilterTypeIndex == 1)
                {
                    newFilter.FromDate = FromDate;
                    newFilter.ToDate = ToDate.AddDays(1);
                }
                else if (DateFilterTypeIndex == 0)
                {
                    newFilter.LastType = SelectedLastTimeSpan?.Type;
                }

                MessagingCenter.Send(this, nameof(FilterAppliedMessage), new FilterAppliedMessage()
                {
                    NewFilter = newFilter,
                });
            });

            ResetDateFilterCommand = new Command(() =>
            {
                ResetDate();
            });

            ResetBarcodeFormatsCommand = new Command(() =>
            {
                ResetBarcodeFormats();
            });
        }

        private void InitBarcodeFormats()
        {
            AvailableBarcodeFormats = new ObservableRangeCollection<ZXingBarcodeFormat>();
            AvailableBarcodeFormats
                .AddRange(Enum.GetValues(typeof(BarcodeFormat))
                    .OfType<BarcodeFormat>()
                    .Select(v => new ZXingBarcodeFormat() { Format = v }));

            SelectedBarcodeFormats = new ObservableRangeCollection<object>();
        }

        private void InitLastTimeSpans()
        {
            AvailableLastTimeSpans = new ObservableRangeCollection<LastTimeSpan>();
            AvailableLastTimeSpans
                .AddRange(Enum.GetValues(typeof(LastTimeType))
                    .OfType<LastTimeType>()
                    .Select(v => new LastTimeSpan() { Type = v }));
        }

        private void ResetFilter()
        {
            ResetDate();
            ResetBarcodeFormats();
        }

        private void ResetDate()
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

            SelectedLastTimeSpan = null;
        }

        private void ResetBarcodeFormats()
        {
            SelectedBarcodeFormats.Clear();
        }

        public int DateFilterTypeIndex { get; set; }

        public ICommand ApplyFilterCommand { get; }

        public ObservableRangeCollection<ZXingBarcodeFormat> AvailableBarcodeFormats { get; set; }
        public ObservableRangeCollection<object> SelectedBarcodeFormats { get; set; }

        public ObservableRangeCollection<LastTimeSpan> AvailableLastTimeSpans { get; set; }
        public LastTimeSpan SelectedLastTimeSpan { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public ICommand ResetDateFilterCommand { get; }
        public ICommand ResetBarcodeFormatsCommand { get; }
    }
}

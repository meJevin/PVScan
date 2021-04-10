using MvvmHelpers;
using PVScan.Mobile.DAL;
using PVScan.Mobile.Models;
using PVScan.Mobile.Services.Interfaces;
using PVScan.Mobile.ViewModels.Messages.Filtering;
using System;
using System.Collections.Generic;
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

    public class FilterPageViewModel : BaseViewModel
    {
        readonly IBarcodesRepository BarcodesRepository;

        public FilterPageViewModel(IBarcodesRepository barcodesRepository)
        {
            BarcodesRepository = barcodesRepository;

            InitBarcodeFormats();
            InitLastTimeSpans();

            // Todo: this is not really good to be honest
            ResetFilter().GetAwaiter().GetResult();

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

            BarcodeFormatItemTappedCommand = new Command((object newFormatObj) =>
            {
                if (SelectedBarcodeFormats.Contains(newFormatObj))
                {
                    SelectedBarcodeFormats.Remove(newFormatObj);
                    return;
                }

                SelectedBarcodeFormats.Add(newFormatObj);
            });

            LastTimeSpanItemTappedCommand = new Command((object newLastTimeSpanObj) =>
            {
                LastTimeSpan newLastTimeSpan = newLastTimeSpanObj as LastTimeSpan;

                if (newLastTimeSpan == SelectedLastTimeSpan)
                {
                    SelectedLastTimeSpan = null;
                    return;
                }

                SelectedLastTimeSpan = newLastTimeSpan;
            });

            ResetDateFilterCommand = new Command(async () =>
            {
                await ResetDate();
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

        private async Task ResetFilter()
        {
            await ResetDate();
            ResetBarcodeFormats();
        }

        private async Task ResetDate()
        {
            // Init min max dates from current DB
            DateTime minDate = DateTime.MinValue;
            DateTime maxDate = DateTime.MaxValue;

            var barcodes = await BarcodesRepository.GetAll();

            try
            {
                minDate = barcodes.Min(b => b.ScanTime);
                maxDate = barcodes.Max(b => b.ScanTime);
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

        public ICommand ApplyFilterCommand { get; }

        public int DateFilterTypeIndex { get; set; }

        public ICommand BarcodeFormatItemTappedCommand { get; }
        public ObservableRangeCollection<ZXingBarcodeFormat> AvailableBarcodeFormats { get; set; }
        public ObservableRangeCollection<object> SelectedBarcodeFormats { get; set; }

        public ICommand LastTimeSpanItemTappedCommand { get; }
        public ObservableRangeCollection<LastTimeSpan> AvailableLastTimeSpans { get; set; }
        public LastTimeSpan SelectedLastTimeSpan { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public ICommand ResetDateFilterCommand { get; }
        public ICommand ResetBarcodeFormatsCommand { get; }
    }
}

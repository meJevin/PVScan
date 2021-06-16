using MvvmHelpers;
using PVScan.Core.Models;
using PVScan.Core.Services.Interfaces;
using PVScan.Desktop.WPF.ViewModels.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZXing;

namespace PVScan.Desktop.WPF.ViewModels
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

            InitDefaultFilter();

            // Todo: this is not really good to be honest
            ResetFilter().GetAwaiter().GetResult();

            ApplyFilterCommand = new Command(() =>
            {
                var newFilter = new Filter()
                {
                    BarcodeFormats = SelectedBarcodeFormats
                                            .Select(o => ((ZXingBarcodeFormat)o).Format)
                                            .ToList(),
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

                CurrentFilter = newFilter;

                ToggleApplyFilterEnabled();

                MessagingCenter.Send(this, nameof(FilterAppliedMessage), new FilterAppliedMessage()
                {
                    NewFilter = newFilter,
                });
            });

            BarcodeFormatItemTappedCommand = new Command((object newFormatObj) =>
            {
                ToggleApplyFilterEnabled();
            });

            LastTimeSpanItemTappedCommand = new Command((object newLastTimeSpanObj) =>
            {
                LastTimeSpan newLastTimeSpan = newLastTimeSpanObj as LastTimeSpan;

                if (newLastTimeSpan == SelectedLastTimeSpan)
                {
                    SelectedLastTimeSpan = null;
                    ToggleApplyFilterEnabled();
                    return;
                }

                ToggleApplyFilterEnabled();
            });

            ResetDateFilterCommand = new Command(async () =>
            {
                await ResetDate();
            });

            ResetBarcodeFormatsCommand = new Command(() =>
            {
                ResetBarcodeFormats();
            });

            SwitchToDateRange = new Command(() =>
            {
                DateFilterTypeIndex = 1;

                ToggleApplyFilterEnabled();
            });

            SwitchToDateLatest = new Command(() =>
            {
                DateFilterTypeIndex = 0;

                ToggleApplyFilterEnabled();
            });

            PropertyChanged += FilterPageViewModel_PropertyChanged;
        }

        private void FilterPageViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ToggleApplyFilterEnabled();
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

        private void InitDefaultFilter()
        {
            CurrentFilter = Filter.Default();
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

            DateFilterTypeIndex = 0;

            ToggleApplyFilterEnabled();
        }

        private void ResetBarcodeFormats()
        {
            SelectedBarcodeFormats.Clear();

            ToggleApplyFilterEnabled();
        }

        private bool AppliedAndCurrentFilterSame()
        {
            if (DateFilterTypeIndex == 0)
            {
                // Last day, weeek, month ...
                if ((CurrentFilter.LastType == null &&
                    SelectedLastTimeSpan != null) ||
                    (CurrentFilter.LastType != null &&
                    SelectedLastTimeSpan == null))
                {
                    // One is selected, the other is not
                    return false;
                }

                if (CurrentFilter.LastType != null &&
                    SelectedLastTimeSpan != null &&
                    CurrentFilter.LastType != SelectedLastTimeSpan.Type)
                {
                    // They don't match
                    return false;
                }
            }
            else if (DateFilterTypeIndex == 1)
            {
                // Range
                if (CurrentFilter.FromDate == null ||
                    CurrentFilter.ToDate == null ||
                    CurrentFilter.FromDate != FromDate ||
                    CurrentFilter.ToDate != ToDate.AddDays(1))
                {
                    return false;
                }
            }

            // Selected barcode type count doesn't match
            if (CurrentFilter.BarcodeFormats.Count() != SelectedBarcodeFormats.Count)
            {
                return false;
            }

            for (int i = 0; i < SelectedBarcodeFormats.Count; ++i)
            {
                // Selected barcodes do not match
                if (SelectedBarcodeFormats
                    .FirstOrDefault(obj => (obj as ZXingBarcodeFormat).Format == CurrentFilter.BarcodeFormats[i]) == null)
                {
                    return false;
                }
            }

            return true;
        }

        public void ToggleApplyFilterEnabled()
        {
            if (AppliedAndCurrentFilterSame())
            {
                ApplyFilterCommandEnabled = false;
                return;
            }

            ApplyFilterCommandEnabled = true;
        }

        public void SetStateFromCurrentFilter()
        {
            if (CurrentFilter.FromDate != null &&
                CurrentFilter.ToDate != null)
            {
                // Range
                DateFilterTypeIndex = 1;
            }
            else
            {
                // Last day, week, month
                DateFilterTypeIndex = 0;

                if (CurrentFilter.LastType != null)
                {
                    SelectedLastTimeSpan = AvailableLastTimeSpans.First(ts => ts.Type == CurrentFilter.LastType);
                }
            }

            // Grab selected barcode formats from current filter
            SelectedBarcodeFormats.Clear();
            foreach (var format in CurrentFilter.BarcodeFormats)
            {
                SelectedBarcodeFormats.Add(AvailableBarcodeFormats.First(bf => bf.Format == format));
            }

            ToggleApplyFilterEnabled();
        }

        public bool ApplyFilterCommandEnabled { get; set; }
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

        public ICommand SwitchToDateRange { get; }
        public ICommand SwitchToDateLatest { get; }

        private Filter CurrentFilter { get; set; }
    }
}

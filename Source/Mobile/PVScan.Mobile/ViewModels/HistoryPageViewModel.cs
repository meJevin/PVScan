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
    public class HistoryPageViewModel : BaseViewModel
    {
        readonly IBarcodesRepository BarcodesRepository;
        readonly IBarcodesFilter FilterService;

        public HistoryPageViewModel(IBarcodesRepository barcodesRepository,
            IBarcodesFilter filterService)
        {
            BarcodesRepository = barcodesRepository;
            FilterService = filterService;

            Barcodes = new ObservableRangeCollection<Barcode>();
            BarcodesPaged = new ObservableRangeCollection<Barcode>();

            SelectedBarcodes = new ObservableCollection<object>();

            MessagingCenter.Subscribe(this, nameof(BarcodeScannedMessage),
                async (ScanPageViewModel vm, BarcodeScannedMessage args) =>
                {
                    var tempEnumerable = new List<Barcode>() { args.ScannedBarcode };
                    if (CurrentFilter != null)
                    {
                        tempEnumerable = FilterService
                                            .Filter(tempEnumerable, CurrentFilter)
                                            .ToList();
                    }

                    if (!String.IsNullOrEmpty(Search))
                    {
                        tempEnumerable = FilterService
                                            .Search(tempEnumerable, Search)
                                            .ToList();
                    }

                    var result = tempEnumerable.FirstOrDefault();
                    if (result != null)
                    {
                        Barcodes.Insert(0, result);
                        BarcodesPaged.Insert(0, result);
                    }
                });

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

            ClearSearchCommand = new Command(async () =>
            {
                Search = "";
            });

            LoadNextPage = new Command(async () =>
            {
                BarcodesPaged.AddRange(Barcodes.Skip(PageCount * PageSize).Take(PageSize));

                ++PageCount;
            });

            // Todo: Add settings to set which parts of barcode object are copied
            CopyBarcodeToClipboardCommand = new Command(async (object barcodeObject) =>
            {
                Barcode barcode = barcodeObject as Barcode;

                if (barcode == null)
                {
                    return;
                }

                // Todo: make this a service!
                await Clipboard.SetTextAsync(barcode.Text);

                // Todo: this too!
                HapticFeedback.Perform(HapticFeedbackType.LongPress);

                BarcodeCopiedToClipboard?.Invoke(this, barcode);
            });

            SelectBarcodeCommand = new Command(async (object barcodeObject) =>
            {
                Barcode barcode = barcodeObject as Barcode;

                if (barcode == null)
                {
                    return;
                }

                SelectedBarcode = barcode;
            });

            DeleteBarcodeCommand = new Command(async (object barcodeObject) =>
            {
                Barcode barcode = barcodeObject as Barcode;

                if (barcode == null)
                {
                    return;
                }

                await BarcodesRepository.Delete(barcode);

                BarcodesPaged.Remove(barcode);
                Barcodes.Remove(barcode);
            });

            StartEditCommand = new Command(() =>
            {
                IsEditing = true;
            });

            DoneEditCommand = new Command(() =>
            {
                IsEditing = false;
                SelectedBarcodes.Clear();
            });

            DeleteSelectedBarcodesCommand = new Command(async () =>
            {
                var sb = SelectedBarcodes.Select(b => b as Barcode);

                foreach (var b in sb)
                {
                    await barcodesRepository.Delete(b);

                    Barcodes.Remove(b);

                    if (BarcodesPaged.Contains(b))
                    {
                        BarcodesPaged.Remove(b);
                    }
                }

                SelectedBarcodes.Clear();
            });
        }

        public async Task LoadBarcodesFromDB()
        {
            if (IsLoading)
            {
                return;
            }

            Barcodes.Clear();
            BarcodesPaged.Clear();
            SelectedBarcodes.Clear();

            PageCount = 1;

            IsLoading = true;

            IEnumerable<Barcode> dbBarcodes = null;

            dbBarcodes = await BarcodesRepository.GetAll();

            if (CurrentFilter != null)
            {
                dbBarcodes = FilterService.Filter(dbBarcodes, CurrentFilter);
            }

            if (!String.IsNullOrEmpty(Search))
            {
                dbBarcodes = FilterService.Search(dbBarcodes, Search);
            }

            Barcodes.AddRange(dbBarcodes.OrderByDescending(b => b.ScanTime));
            BarcodesPaged.AddRange(Barcodes.Take(PageSize));

            IsLoading = false;
        }

        public string Search { get; set; }

        public Filter CurrentFilter { get; set; }

        public ObservableRangeCollection<Barcode> Barcodes { get; set; }
        public ObservableRangeCollection<Barcode> BarcodesPaged { get; set; }

        // How many pages have we loaded in the list?
        private int PageCount { get; set; }
        // How many items per page?
        private int PageSize { get; set; } = 50;

        // Remaining item threshold
        public int RemainingBarcodesThreshold { get; set; } = 3;
        public ICommand LoadNextPage { get; set; }


        public bool IsLoading { get; set; }
        public bool IsRefresing { get; set; }
        public ICommand RefreshCommand { get; set; }


        public ICommand SearchCommand { get; set; }
        public ICommand ClearSearchCommand { get; set; }


        public ICommand CopyBarcodeToClipboardCommand { get; set; }
        public event EventHandler<Barcode> BarcodeCopiedToClipboard;


        public ICommand SelectBarcodeCommand { get; set; }
        public Barcode SelectedBarcode { get; set; }


        public ICommand DeleteBarcodeCommand { get; set; }


        public bool IsEditing { get; set; }
        public ICommand StartEditCommand { get; set; }
        public ICommand DoneEditCommand { get; set; }
        public ICommand DeleteSelectedBarcodesCommand { get; set; }

        public ObservableCollection<object> SelectedBarcodes { get; set; }
    }
}

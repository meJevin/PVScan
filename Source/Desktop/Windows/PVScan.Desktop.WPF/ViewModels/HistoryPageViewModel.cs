using MvvmHelpers;
using PVScan.Core.Models;
using PVScan.Core.Services.Interfaces;
using PVScan.Desktop.WPF.ViewModels.Messages;
using PVScan.Desktop.WPF.ViewModels.Messages.Scanning;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PVScan.Desktop.WPF.ViewModels
{
    public class HistoryPageViewModel : BaseViewModel
    {
        readonly IBarcodesFilter FilterService;
        readonly IBarcodeSorter SorterService;
        readonly IBarcodesRepository BarcodesRepository;

        public HistoryPageViewModel(
            IBarcodesRepository barcodesRepository,
            IBarcodesFilter filterService,
            IBarcodeSorter sorterService)
        {
            FilterService = filterService;
            SorterService = sorterService;
            BarcodesRepository = barcodesRepository;

            Barcodes = new ObservableRangeCollection<Barcode>();
            BarcodesPaged = new ObservableRangeCollection<Barcode>();

            LoadNextPage = new Command(async () =>
            {
                var newBarcodes = Barcodes.Skip(PageCount * PageSize).Take(PageSize);

                if (newBarcodes.Count() == 0)
                {
                    return;
                }

                foreach (var b in newBarcodes)
                {
                    BarcodesPaged.Add(b);
                }

                ++PageCount;
            });

            SearchCommand = new Command(async () =>
            {
                await LoadBarcodesFromDB();
            });

            FavoriteCommand = new Command(async (object barcodeObject) =>
            {
                Barcode barcode = barcodeObject as Barcode;

                if (barcode == null)
                {
                    return;
                }

                barcode.Favorite = !barcode.Favorite;
                await BarcodesRepository.Update(barcode);

                var indxPaged = BarcodesPaged.IndexOf(barcode);
                var indxTotal = Barcodes.IndexOf(barcode);

                if (indxPaged != -1)
                {
                    BarcodesPaged[indxPaged] = null;
                    BarcodesPaged[indxPaged] = barcode;
                }

                if (indxTotal != -1)
                {
                    Barcodes[indxPaged] = null;
                    Barcodes[indxTotal] = barcode;
                }
            });


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
                        var tempList = Barcodes.ToList();
                        tempList.Add(result);

                        var newListSorted = (await SorterService.Sort(tempList, CurrentSorting)).ToList();

                        var insertedIndex = newListSorted.IndexOf(result);

                        Barcodes.Insert(insertedIndex, result);
                        int lastPagedIndex = BarcodesPaged.Count;
                        if (insertedIndex <= lastPagedIndex)
                        {
                            BarcodesPaged.Insert(insertedIndex, result);
                        }
                    }
                });

            MessagingCenter.Subscribe(this, nameof(FilterAppliedMessage),
                async (FilterPageViewModel vm, FilterAppliedMessage args) =>
                {
                    CurrentFilter = args.NewFilter;

                    await LoadBarcodesFromDB();
                });

            MessagingCenter.Subscribe(this, nameof(SortingAppliedMessage),
                async (SortingPageViewModel vm, SortingAppliedMessage args) =>
                {
                    CurrentSorting = args.NewSorting;

                    await LoadBarcodesFromDB();
                });

            _ = LoadBarcodesFromDB();
        }

        public async Task LoadBarcodesFromDB()
        {
            if (IsLoading)
            {
                return;
            }

            Barcodes.Clear();
            BarcodesPaged.Clear();

            PageCount = 0;
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

            if (CurrentSorting != null)
            {
                dbBarcodes = await SorterService.Sort(dbBarcodes, CurrentSorting);
            }

            foreach (var b in dbBarcodes)
            {
                Barcodes.Add(b);
            }

            foreach (var b in Barcodes.Take(PageSize))
            {
                BarcodesPaged.Add(b);
            }

            PageCount = 1;
            IsLoading = false;
        }

        public string Search { get; set; }

        public Filter CurrentFilter { get; set; }
        public Sorting CurrentSorting { get; set; } = Sorting.Default();


        public ObservableCollection<Barcode> Barcodes { get; set; }
        public ObservableCollection<Barcode> BarcodesPaged { get; set; }


        public int PageCount { get; set; }
        private int PageSize { get; set; } = 35;

        public ICommand LoadNextPage { get; set; }

        public ICommand SearchCommand { get; set; }

        public bool IsLoading { get; set; }

        public ICommand FavoriteCommand { get; set; }
    }
}

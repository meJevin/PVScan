using MvvmHelpers;
using PVScan.Core.Models;
using PVScan.Core.Services.Interfaces;
using PVScan.Desktop.WPF.ViewModels.Messages;
using PVScan.Desktop.WPF.ViewModels.Messages.Barcodes;
using PVScan.Desktop.WPF.ViewModels.Messages.Scanning;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

            SelectedBarcodes = new ObservableCollection<Barcode>();

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

            StartEditCommand = new Command(() =>
            {
                IsEditing = true;
            });

            DoneEditCommand = new Command(() =>
            {
                SelectedBarcodes.Clear();
                IsEditing = false;
            });

            DeleteSelectedBarcodesCommand = new Command(async () =>
            {
                var selectedGUIDS = new List<string>();

                foreach (var b in SelectedBarcodes)
                {
                    selectedGUIDS.Add(b.GUID);
                }

                foreach (var guid in selectedGUIDS)
                {
                    var barcode = Barcodes.FirstOrDefault(b => b.GUID == guid);

                    await barcodesRepository.Delete(barcode);

                    if (Barcodes.Contains(barcode))
                    {
                        Barcodes.Remove(barcode);
                    }

                    if (BarcodesPaged.Contains(barcode))
                    {
                        BarcodesPaged.Remove(barcode);
                    }
                }

                SelectedBarcodes.Clear();
            });

            Barcodes.CollectionChanged += Barcodes_CollectionChanged;

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

            MessagingCenter.Subscribe(this, nameof(BarcodeDeletedMessage),
                async (MainWindowViewModel vm, BarcodeDeletedMessage args) =>
                {
                    if (Barcodes.Contains(args.DeletedBarcode))
                    {
                        Barcodes.Remove(args.DeletedBarcode);
                    }

                    if (BarcodesPaged.Contains(args.DeletedBarcode))
                    {
                        BarcodesPaged.Remove(args.DeletedBarcode);
                    }
                });

            _ = LoadBarcodesFromDB();
        }

        private void Barcodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            MessagingCenter.Send(this, 
                nameof(HistoryPageBarcodesCollectionChanged), 
                new HistoryPageBarcodesCollectionChanged() { Args = e });
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


        public bool IsEditing { get; set; }
        public ICommand StartEditCommand { get; set; }
        public ICommand DoneEditCommand { get; set; }
        public ICommand DeleteSelectedBarcodesCommand { get; set; }

        public ObservableCollection<Barcode> SelectedBarcodes { get; set; }
        public Barcode SelectedBarcode { get; set; }
    }
}

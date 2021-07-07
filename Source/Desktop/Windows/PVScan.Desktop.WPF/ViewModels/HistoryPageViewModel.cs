using MvvmHelpers;
using PVScan.Core.Models;
using PVScan.Core.Models.API;
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
using System.Windows;
using System.Windows.Input;

namespace PVScan.Desktop.WPF.ViewModels
{
    public class HistoryPageViewModel : BaseViewModel
    {
        readonly IBarcodesFilter FilterService;
        readonly IBarcodeSorter SorterService;
        readonly IBarcodesRepository BarcodesRepository;
        readonly IPVScanAPI PVScanAPI;
        readonly IAPIBarcodeHub BarcodeHub;

        public HistoryPageViewModel(
            IBarcodesRepository barcodesRepository,
            IBarcodesFilter filterService,
            IBarcodeSorter sorterService,
            IPVScanAPI pVScanAPI,
            IAPIBarcodeHub barcodeHub)
        {
            FilterService = filterService;
            SorterService = sorterService;
            BarcodesRepository = barcodesRepository;
            PVScanAPI = pVScanAPI;
            BarcodeHub = barcodeHub;

            Barcodes = new ObservableRangeCollection<Barcode>();
            BarcodesPaged = new ObservableRangeCollection<Barcode>();

            SelectedBarcodes = new ObservableCollection<Barcode>();

            BarcodeHub.OnDeleted += BarcodeHub_OnDeleted;
            BarcodeHub.OnUpdated += BarcodeHub_OnUpdated;
            BarcodeHub.OnScanned += BarcodeHub_OnScanned;

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

                var req = new UpdatedBarcodeRequest()
                {
                    GUID = barcode.GUID,
                    Latitude = barcode.ScanLocation?.Latitude,
                    Longitude = barcode.ScanLocation?.Longitude,
                    Favorite = barcode.Favorite,
                };

                if (await PVScanAPI.UpdatedBarcode(req) != null)
                {
                    await BarcodeHub.Updated(req);
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
                    var barcode = Barcodes.FirstOrDefault(b => { if (b == null) return false; return b.GUID == guid; });

                    await barcodesRepository.Delete(barcode);

                    MessagingCenter.Send(this, nameof(BarcodeDeletedMessage),
                        new BarcodeDeletedMessage() { DeletedBarcode = barcode });

                    if (Barcodes.Contains(barcode))
                    {
                        Barcodes.Remove(barcode);
                    }

                    if (BarcodesPaged.Contains(barcode))
                    {
                        BarcodesPaged.Remove(barcode);
                    }

                    var req = new DeletedBarcodeRequest()
                    {
                        GUID = barcode.GUID,
                    };

                    if (await PVScanAPI.DeletedBarcode(req) != null)
                    {
                        await BarcodeHub.Deleted(req);
                    }
                }

                SelectedBarcodes.Clear();
            });

            Barcodes.CollectionChanged += Barcodes_CollectionChanged;

            MessagingCenter.Subscribe(this, nameof(BarcodeScannedMessage),
                async (ScanPageViewModel vm, BarcodeScannedMessage args) =>
                {
                    await BarcodeScanned(args);
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

            MessagingCenter.Subscribe(this, nameof(LocationSpecifiedMessage),
                async (MapPageViewModel vm, LocationSpecifiedMessage args) =>
                {
                    await Application.Current.Dispatcher.InvokeAsync(async () =>
                    {
                        var indxPaged = BarcodesPaged.IndexOf(args.Barcode);
                        var indxTotal = Barcodes.IndexOf(args.Barcode);

                        if (indxPaged != -1)
                        {
                            BarcodesPaged.RemoveAt(indxPaged);
                            BarcodesPaged.Insert(indxPaged, args.Barcode);
                        }

                        if (indxTotal != -1)
                        {
                            Barcodes.RemoveAt(indxTotal);
                            Barcodes.Insert(indxTotal, args.Barcode);
                        }
                    });
                });

            _ = LoadBarcodesFromDB();
        }

        private void Barcodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            MessagingCenter.Send(this, 
                nameof(HistoryPageBarcodesCollectionChanged), 
                new HistoryPageBarcodesCollectionChanged() { Args = e });
        }

        private async Task BarcodeScanned(BarcodeScannedMessage args)
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
        }

        private async void BarcodeHub_OnUpdated(object sender, UpdatedBarcodeRequest req)
        {
            var localBarcode = await BarcodesRepository.FindByGUID(req.GUID);

            if (localBarcode == null)
            {
                return;
            }

            localBarcode.ScanLocation = new Coordinate()
            {
                Latitude = req.Latitude,
                Longitude = req.Longitude,
            };
            localBarcode.Favorite = req.Favorite;

            await BarcodesRepository.Update(localBarcode);

            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                var indxPaged = BarcodesPaged.IndexOf(localBarcode);
                var indxTotal = Barcodes.IndexOf(localBarcode);

                if (indxPaged != -1)
                {
                    BarcodesPaged.RemoveAt(indxPaged);
                    BarcodesPaged.Insert(indxPaged, localBarcode);
                }

                if (indxTotal != -1)
                {
                    Barcodes.RemoveAt(indxTotal);
                    Barcodes.Insert(indxTotal, localBarcode);
                }
            });
        }

        private async void BarcodeHub_OnDeleted(object sender, DeletedBarcodeRequest req)
        {
            Barcode barcode = await BarcodesRepository.FindByGUID(req.GUID);

            if (barcode == null)
            {
                return;
            }

            await BarcodesRepository.Delete(barcode);

            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                if (BarcodesPaged.Contains(barcode))
                {
                    BarcodesPaged.Remove(barcode);
                }

                if (Barcodes.Contains(barcode))
                {
                    Barcodes.Remove(barcode);
                }
            });
        }

        private async void BarcodeHub_OnScanned(object sender, ScannedBarcodeRequest b)
        {
            Barcode newBarcode = new Barcode()
            {
                Favorite = b.Favorite,
                Format = b.Format,
                GUID = b.GUID,
                Hash = b.Hash,
                ScanLocation = null,
                ScanTime = b.ScanTime,
                Text = b.Text,
            };

            if (b.Latitude.HasValue && b.Longitude.HasValue)
            {
                newBarcode.ScanLocation = new Coordinate()
                {
                    Latitude = b.Latitude,
                    Longitude = b.Longitude
                };
            }

            newBarcode = await BarcodesRepository.Save(newBarcode);

            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await BarcodeScanned(new BarcodeScannedMessage() { ScannedBarcode = newBarcode });
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

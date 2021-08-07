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
        readonly IBarcodeSynchronizer Synchronizer;

        public HistoryPageViewModel(
            IBarcodesRepository barcodesRepository,
            IBarcodesFilter filterService,
            IBarcodeSorter sorterService,
            IPVScanAPI pVScanAPI,
            IAPIBarcodeHub barcodeHub,
            IBarcodeSynchronizer synchronizer)
        {
            FilterService = filterService;
            SorterService = sorterService;
            BarcodesRepository = barcodesRepository;
            PVScanAPI = pVScanAPI;
            BarcodeHub = barcodeHub;
            Synchronizer = synchronizer;

            Barcodes = new ObservableRangeCollection<Barcode>();
            BarcodesPaged = new ObservableRangeCollection<Barcode>();

            SelectedBarcodes = new ObservableCollection<Barcode>();

            BarcodeHub.OnScanned += BarcodeHub_OnScanned;
            BarcodeHub.OnDeleted += BarcodeHub_OnDeleted;
            BarcodeHub.OnUpdated += BarcodeHub_OnUpdated;

            BarcodeHub.OnScannedMultiple += BarcodeHub_OnScannedMultiple;
            BarcodeHub.OnDeletedMultiple += BarcodeHub_OnDeletedMultiple;
            BarcodeHub.OnUpdatedMultple += BarcodeHub_OnUpdatedMultiple;

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
                    LastTimeUpdated = barcode.LastUpdateTime,
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
                var sb = SelectedBarcodes.Select(b => b).ToList();
                var toDeleteOnServer = new List<DeletedBarcodeRequest>();
                foreach (var b in sb)
                {
                    Barcodes.Remove(b);
                    BarcodesPaged.Remove(b);

                    MessagingCenter.Send(this, nameof(BarcodeDeletedMessage),
                        new BarcodeDeletedMessage() { DeletedBarcode = b });

                    toDeleteOnServer.Add(new DeletedBarcodeRequest()
                    {
                        GUID = b.GUID,
                    });
                }

                SelectedBarcodes.Clear();

                foreach (var b in sb)
                {
                    await barcodesRepository.Delete(b);
                }

                // Todo: Possibly BarcodesHub wants to go into the PVSCanAPI?
                if (await PVScanAPI.DeletedBarcodeMultiple(toDeleteOnServer) != null)
                {
                    await BarcodeHub.DeletedMultiple(toDeleteOnServer);
                }
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

            Synchronizer.SynchorinizedLocally += Synchronizer_SynchorinizedLocally;
        }

        private void Synchronizer_SynchorinizedLocally(object sender, EventArgs e)
        {
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
            var barcode = await BarcodesRepository.FindByGUID(req.GUID);

            if (barcode == null)
            {
                return;
            }

            if (req.Longitude.HasValue && req.Latitude.HasValue)
            {
                barcode.ScanLocation = new Coordinate()
                {
                    Latitude = req.Latitude,
                    Longitude = req.Longitude,
                };
            }
            barcode.Favorite = req.Favorite;

            await BarcodesRepository.Update(barcode);

            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                var bPaged = BarcodesPaged.FirstOrDefault(b => b.GUID == barcode.GUID);
                var b = Barcodes.FirstOrDefault(b => b.GUID == barcode.GUID);

                if (bPaged != null)
                {
                    var indxPaged = BarcodesPaged.IndexOf(bPaged);
                    BarcodesPaged.RemoveAt(indxPaged);
                    BarcodesPaged.Insert(indxPaged, barcode);
                }

                if (b != null)
                {
                    var indx = Barcodes.IndexOf(b);
                    Barcodes.RemoveAt(indx);
                    Barcodes.Insert(indx, barcode);
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
                var bPaged = BarcodesPaged.FirstOrDefault(b => b.GUID == barcode.GUID);
                var b = Barcodes.FirstOrDefault(b => b.GUID == barcode.GUID);

                if (bPaged != null)
                {
                    BarcodesPaged.Remove(bPaged);
                }

                if (b != null)
                {
                    Barcodes.Remove(b);
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
                LastUpdateTime = b.LastTimeUpdated,
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

        private async void BarcodeHub_OnUpdatedMultiple(object sender, List<UpdatedBarcodeRequest> e)
        {
            List<Barcode> toUpdate = new List<Barcode>();
            foreach (var req in e)
            {
                var localBarcode = await BarcodesRepository.FindByGUID(req.GUID);

                if (localBarcode == null)
                {
                    continue;
                }

                if (req.Longitude.HasValue && req.Latitude.HasValue)
                {
                    localBarcode.ScanLocation = new Coordinate()
                    {
                        Latitude = req.Latitude,
                        Longitude = req.Longitude,
                    };
                }
                localBarcode.Favorite = req.Favorite;

                toUpdate.Add(localBarcode);
            }

            await BarcodesRepository.Update(toUpdate);

            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                foreach (var barcode in toUpdate)
                {
                    var bPaged = BarcodesPaged.FirstOrDefault(b => b.GUID == barcode.GUID);
                    var b = Barcodes.FirstOrDefault(b => b.GUID == barcode.GUID);

                    if (bPaged != null)
                    {
                        var indxPaged = BarcodesPaged.IndexOf(bPaged);
                        BarcodesPaged.RemoveAt(indxPaged);
                        BarcodesPaged.Insert(indxPaged, bPaged);
                    }

                    if (b != null)
                    {
                        var indx = Barcodes.IndexOf(b);
                        Barcodes.RemoveAt(indx);
                        Barcodes.Insert(indx, b);
                    }
                }
            });
        }

        private async void BarcodeHub_OnDeletedMultiple(object sender, List<DeletedBarcodeRequest> e)
        {
            List<Barcode> toDelete = new List<Barcode>();
            foreach (var req in e)
            {
                Barcode barcode = await BarcodesRepository.FindByGUID(req.GUID);

                if (barcode == null)
                {
                    continue;
                }

                toDelete.Add(barcode);
            }

            await BarcodesRepository.Delete(toDelete);

            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                foreach (var barcode in toDelete)
                {
                    var bPaged = BarcodesPaged.FirstOrDefault(b => b.GUID == barcode.GUID);
                    var b = Barcodes.FirstOrDefault(b => b.GUID == barcode.GUID);

                    if (bPaged != null)
                    {
                        BarcodesPaged.Remove(bPaged);
                    }

                    if (b != null)
                    {
                        Barcodes.Remove(b);
                    }
                }
            });
        }

        private async void BarcodeHub_OnScannedMultiple(object sender, List<ScannedBarcodeRequest> e)
        {
            var toSave = new List<Barcode>();
            foreach (var b in e)
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
                    LastUpdateTime = b.LastTimeUpdated,
                };

                if (b.Latitude.HasValue && b.Longitude.HasValue)
                {
                    newBarcode.ScanLocation = new Coordinate()
                    {
                        Latitude = b.Latitude,
                        Longitude = b.Longitude
                    };
                }

                toSave.Add(newBarcode);
            }

            await BarcodesRepository.Save(toSave);

            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                foreach (var b in toSave)
                {
                    await BarcodeScanned(new BarcodeScannedMessage() { ScannedBarcode = b });
                }
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

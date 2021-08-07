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

                await UpdateBarcodesInUI(new List<Barcode> { barcode });

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
                var toDelete = SelectedBarcodes.Select(b => b).ToList();
                var toDeleteOnServer = new List<DeletedBarcodeRequest>();
                foreach (var barcode in toDelete)
                {
                    MessagingCenter.Send(this, nameof(BarcodeDeletedMessage),
                        new BarcodeDeletedMessage() { DeletedBarcode = barcode });

                    toDeleteOnServer.Add(new DeletedBarcodeRequest()
                    {
                        GUID = barcode.GUID,
                    });
                }

                await DeleteBarcodesFromUI(toDelete);

                SelectedBarcodes.Clear();

                foreach (var b in toDelete)
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
                    await AddBarcodesToUI(new List<Barcode> { args.ScannedBarcode });
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
                    await DeleteBarcodesFromUI(new List<Barcode> { args.DeletedBarcode });
                });

            MessagingCenter.Subscribe(this, nameof(LocationSpecifiedMessage),
                async (MapPageViewModel vm, LocationSpecifiedMessage args) =>
                {
                    await UpdateBarcodesInUI(new List<Barcode> { args.Barcode });
                });

            _ = LoadBarcodesFromDB();

            Synchronizer.Synchronized += Synchronizer_Synchronized;
        }

        private async void Synchronizer_Synchronized(object sender, SynchronizeResponse e)
        {
            if (e.ToAddLocaly.Count() > 0)
            {
                await AddBarcodesToUI(e.ToAddLocaly.ToList());
            }
            
            if (e.ToUpdateLocaly.Count() > 0)
            {
                await UpdateBarcodesInUI(e.ToUpdateLocaly.ToList());
            }
        }

        private void Barcodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // This is sent over to MapPage so it can add points on map
            MessagingCenter.Send(this, 
                nameof(HistoryPageBarcodesCollectionChanged), 
                new HistoryPageBarcodesCollectionChanged() { Args = e });
        }

        private async Task AddBarcodesToUI(List<Barcode> toAdd)
        {
            var filtered = toAdd;
            if (CurrentFilter != null)
            {
                filtered = FilterService
                                    .Filter(filtered, CurrentFilter)
                                    .ToList();
            }

            if (!String.IsNullOrEmpty(Search))
            {
                filtered = FilterService
                                    .Search(filtered, Search)
                                    .ToList();
            }

            if (filtered.Any())
            {
                var merged = Barcodes.ToList();
                foreach (var b in filtered)
                {
                    merged.Add(b);
                }


                var mergedSorted = (await SorterService.Sort(merged, CurrentSorting)).ToList();

                foreach (var b in filtered)
                {
                    var mergedIndex = mergedSorted.IndexOf(b);

                    var lowerBarcodeIndex = mergedIndex + 1;
                    while (lowerBarcodeIndex < mergedSorted.Count - 1)
                    {
                        var foundInOriginal = Barcodes.FirstOrDefault(b => b.GUID == mergedSorted[lowerBarcodeIndex].GUID);
                        if (foundInOriginal != null)
                        {
                            lowerBarcodeIndex = Barcodes.IndexOf(foundInOriginal);
                            break;
                        }

                        ++lowerBarcodeIndex;
                    }

                    var upperBarcodeIndex = mergedIndex - 1;
                    while (upperBarcodeIndex > 0)
                    {
                        var foundInOriginal = Barcodes.FirstOrDefault(b => b.GUID == mergedSorted[upperBarcodeIndex].GUID);
                        if (foundInOriginal != null)
                        {
                            upperBarcodeIndex = Barcodes.IndexOf(foundInOriginal);
                            break;
                        }

                        --upperBarcodeIndex;
                    }

                    var resultIndex = lowerBarcodeIndex;
                    if (upperBarcodeIndex < 0)
                    {
                        // Very top
                        resultIndex = 0;
                    }
                    else if (lowerBarcodeIndex > mergedSorted.Count - 1)
                    {
                        // Very bottom
                        resultIndex = Barcodes.Count - 1;
                    }

                    Barcodes.Insert(resultIndex, b);
                    int lastPagedIndex = BarcodesPaged.Count;
                    if (resultIndex <= lastPagedIndex)
                    {
                        BarcodesPaged.Insert(resultIndex, b);
                    }
                }
            }
        }

        private async Task UpdateBarcodesInUI(List<Barcode> toUpdate)
        {
            foreach (var barcode in toUpdate)
            {
                var bPaged = BarcodesPaged.FirstOrDefault(b => b.GUID == barcode.GUID);
                var b = Barcodes.FirstOrDefault(b => b.GUID == barcode.GUID);

                if (bPaged != null)
                {
                    var indxPaged = BarcodesPaged.IndexOf(bPaged);
                    BarcodesPaged[indxPaged] = null;
                    BarcodesPaged[indxPaged] = barcode;
                }

                if (b != null)
                {
                    var indx = Barcodes.IndexOf(b);
                    Barcodes[indx] = null;
                    Barcodes[indx] = barcode;
                }
            }
        }

        private async Task DeleteBarcodesFromUI(List<Barcode> toDelete)
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

            await UpdateBarcodesInUI(new List<Barcode> { barcode });
        }

        private async void BarcodeHub_OnDeleted(object sender, DeletedBarcodeRequest req)
        {
            Barcode barcode = await BarcodesRepository.FindByGUID(req.GUID);

            if (barcode == null)
            {
                return;
            }

            await BarcodesRepository.Delete(barcode);
            await DeleteBarcodesFromUI(new List<Barcode> { barcode });
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

            await AddBarcodesToUI(new List<Barcode> { newBarcode });
        }

        private async void BarcodeHub_OnUpdatedMultiple(object sender, List<UpdatedBarcodeRequest> e)
        {
            var toUpdate = new List<Barcode>();
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
            await UpdateBarcodesInUI(toUpdate);
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
            await DeleteBarcodesFromUI(toDelete);
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
            await AddBarcodesToUI(toSave);
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

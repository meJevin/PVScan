using Microsoft.EntityFrameworkCore;
using MvvmHelpers;
using PVScan.Mobile.Converters;
using PVScan.Core.DAL;
using PVScan.Core.Models;
using PVScan.Core.Models.API;
using PVScan.Mobile.Services;
using PVScan.Core.Services.Interfaces;
using PVScan.Mobile.ViewModels.Messages;
using PVScan.Mobile.ViewModels.Messages.Filtering;
using PVScan.Mobile.ViewModels.Messages.Scanning;
using PVScan.Mobile.Views;
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
using PVScan.Mobile.Services.Interfaces;

namespace PVScan.Mobile.ViewModels
{
    public class HistoryPageViewModel : BaseViewModel
    {
        readonly IBarcodesRepository BarcodesRepository;
        readonly IBarcodesFilter FilterService;
        readonly IBarcodeSorter SorterService;
        readonly IPopupMessageService PopupMessageService;
        readonly IPVScanAPI PVScanAPI;
        readonly IAPIBarcodeHub BarcodeHub;
        readonly IBarcodeSynchronizer Synchronizer;

        public HistoryPageViewModel(
            IBarcodesRepository barcodesRepository,
            IBarcodesFilter filterService,
            IPopupMessageService popupMessageService,
            IBarcodeSorter sorterService,
            IPVScanAPI pVScanAPI,
            IAPIBarcodeHub barcodeHub,
            IBarcodeSynchronizer synchronizer)
        {
            BarcodesRepository = barcodesRepository;
            FilterService = filterService;
            SorterService = sorterService;
            PopupMessageService = popupMessageService;
            PVScanAPI = pVScanAPI;
            BarcodeHub = barcodeHub;
            Synchronizer = synchronizer;

            Barcodes = new ObservableRangeCollection<Barcode>();
            BarcodesPaged = new ObservableRangeCollection<Barcode>();

            SelectedBarcodes = new ObservableCollection<object>();

            BarcodeHub.OnDeleted += BarcodeHub_OnDeleted;
            BarcodeHub.OnUpdated += BarcodeHub_OnUpdated;
            BarcodeHub.OnScanned += BarcodeHub_OnScanned;

            BarcodeHub.OnScannedMultiple += BarcodeHub_OnScannedMultiple;
            BarcodeHub.OnDeletedMultiple += BarcodeHub_OnDeletedMultiple;
            BarcodeHub.OnUpdatedMultple += BarcodeHub_OnUpdatedMultiple;

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

            MessagingCenter.Subscribe(this, nameof(BarcodeLocationSpecifiedMessage),
                async (SpecifyLocationPageViewModel vm, BarcodeLocationSpecifiedMessage args) =>
                {
                    // This is required to update the map, so that the marker appears
                    var indexTotal = Barcodes.IndexOf(args.UpdatedBarcode);
                    var indexPaged = BarcodesPaged.IndexOf(args.UpdatedBarcode);

                    if (indexTotal != -1)
                    {
                        Barcodes[indexTotal] = args.UpdatedBarcode;
                    }

                    if (indexPaged != -1)
                    {
                        BarcodesPaged[indexPaged] = args.UpdatedBarcode;
                    }
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
                var newBarcodes = Barcodes.Skip(PageCount * PageSize).Take(PageSize);

                if (newBarcodes.Count() == 0)
                {
                    return;
                }

                BarcodesPaged.AddRange(newBarcodes);

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
                await PopupMessageService.ShowMessage("Text copied");
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
                HapticFeedback.Perform(HapticFeedbackType.Click);

                MessagingCenter.Send(this, nameof(BarcodeFavotireToggledMessage),
                    new BarcodeFavotireToggledMessage()
                    {
                        UpdatedBarcode = barcode,
                    });

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

                var req = new DeletedBarcodeRequest()
                {
                    GUID = barcode.GUID,
                };

                if (await PVScanAPI.DeletedBarcode(req) != null)
                {
                    await BarcodeHub.Deleted(req);
                }
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

            // Todo: this can actually be called twice in offline mode when
            // the user is waiting for previous API request (deleted)
            // Look at 
            DeleteSelectedBarcodesCommand = new Command(async () =>
            {
                var sb = SelectedBarcodes.Select(b => b as Barcode).ToList();
                var toDeleteOnServer = new List<DeletedBarcodeRequest>();
                foreach (var b in sb)
                {
                    Barcodes.Remove(b);
                    BarcodesPaged.Remove(b);

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

            Synchronizer.SynchorinizedLocally += Synchronizer_SynchorinizedLocally;
        }

        private void Synchronizer_SynchorinizedLocally(object sender, EventArgs e)
        {
            _ = LoadBarcodesFromDB();
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

            var bPaged = BarcodesPaged.FirstOrDefault(b => b.GUID == barcode.GUID);
            var b = Barcodes.FirstOrDefault(b => b.GUID == barcode.GUID);

            if (bPaged != null)
            {
                var indxPaged = BarcodesPaged.IndexOf(bPaged);
                if (indxPaged != -1)
                {
                    BarcodesPaged[indxPaged] = barcode;
                }
            }

            if (b != null)
            {
                var indx = Barcodes.IndexOf(b);

                if (indx != -1)
                {
                    Barcodes[indx] = barcode;
                }
            }

            if (SelectedBarcode == null ||
                req.GUID != SelectedBarcode.GUID)
            {
                return;
            }

            SelectedBarcode = null;
            await Task.Delay(5);
            SelectedBarcode = barcode;
        }

        private async void BarcodeHub_OnDeleted(object sender, DeletedBarcodeRequest req)
        {
            Barcode barcode = await BarcodesRepository.FindByGUID(req.GUID);

            if (barcode == null)
            {
                return;
            }

            await BarcodesRepository.Delete(barcode);

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

            MessagingCenter.Send(this, nameof(BarcodeScannedMessage), new BarcodeScannedMessage()
            {
                ScannedBarcode = newBarcode,
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

            foreach (var barcode in toUpdate)
            {
                var bPaged = BarcodesPaged.FirstOrDefault(b => b.GUID == barcode.GUID);
                var b = Barcodes.FirstOrDefault(b => b.GUID == barcode.GUID);

                if (bPaged != null)
                {
                    var indxPaged = BarcodesPaged.IndexOf(bPaged);

                    if (indxPaged != -1)
                    {
                        BarcodesPaged[indxPaged] = barcode;
                    }
                }

                if (b != null)
                {
                    var indx = Barcodes.IndexOf(b);

                    if (indx != -1)
                    {
                        Barcodes[indx] = barcode;
                    }
                }

                if (SelectedBarcode != null)
                {
                    var found = e.FirstOrDefault(b => b.GUID == SelectedBarcode.GUID);

                    if (SelectedBarcode == null ||
                        found == null)
                    {
                        continue;
                    }

                    SelectedBarcode = null;
                    await Task.Delay(5);
                    SelectedBarcode = Barcodes[Barcodes.IndexOf(Barcodes.First(b => b.GUID == found.GUID))];
                }
            }
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

            foreach (var b in toSave)
            {
                MessagingCenter.Send(this, nameof(BarcodeScannedMessage), new BarcodeScannedMessage()
                {
                    ScannedBarcode = b,
                });
            }
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
            SelectedBarcode = null;

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

            dbBarcodes = dbBarcodes.ToList();
            Barcodes.AddRange(dbBarcodes);
            BarcodesPaged.AddRange(Barcodes.Take(PageSize));

            PageCount = 1;
            IsLoading = false;
        }

        public string Search { get; set; }

        public Filter CurrentFilter { get; set; }
        public Sorting CurrentSorting { get; set; } = Sorting.Default();

        public ObservableRangeCollection<Barcode> Barcodes { get; set; }
        public ObservableRangeCollection<Barcode> BarcodesPaged { get; set; }


        // How many pages have we loaded in the list?
        public int PageCount { get; set; }
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


        public ICommand FavoriteCommand { get; set; }
        public ICommand SelectBarcodeCommand { get; set; }
        public Barcode SelectedBarcode { get; set; }
        public Barcode NoLocationSelectedBarcode { get; set; }


        public ICommand DeleteBarcodeCommand { get; set; }


        public bool IsEditing { get; set; }
        public ICommand StartEditCommand { get; set; }
        public ICommand DoneEditCommand { get; set; }
        public ICommand DeleteSelectedBarcodesCommand { get; set; }

        public ObservableCollection<object> SelectedBarcodes { get; set; }


        public Barcode HighlightedBarcode { get; set; }
    }
}

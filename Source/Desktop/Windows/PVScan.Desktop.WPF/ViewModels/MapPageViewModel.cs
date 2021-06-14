using PVScan.Core.Models;
using PVScan.Core.Services.Interfaces;
using PVScan.Desktop.WPF.ViewModels.Messages.Barcodes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Desktop.WPF.ViewModels
{
    public class MapPageViewModel : BaseViewModel
    {
        readonly IBarcodesFilter FilterService;
        readonly IBarcodeSorter SorterService;
        readonly IBarcodesRepository BarcodesRepository;

        public MapPageViewModel(
            IBarcodesRepository barcodesRepository,
            IBarcodesFilter filterService,
            IBarcodeSorter sorterService)
        {
            FilterService = filterService;
            SorterService = sorterService;
            BarcodesRepository = barcodesRepository;

            Barcodes = new ObservableCollection<Barcode>();

            MessagingCenter.Subscribe<HistoryPageViewModel, HistoryPageBarcodesCollectionChanged>(this,
                nameof(HistoryPageBarcodesCollectionChanged), (sender, args) => {
                    if (args.Args.Action == NotifyCollectionChangedAction.Add)
                    {
                        foreach (Barcode b in args.Args.NewItems)
                        {
                            Barcodes.Add(b);
                        }
                    }
                    else if (args.Args.Action == NotifyCollectionChangedAction.Remove)
                    {
                        foreach (Barcode b in args.Args.OldItems)
                        {
                            Barcodes.Remove(b);
                        }
                    }
                    else if (args.Args.Action == NotifyCollectionChangedAction.Reset)
                    {
                        Barcodes.Clear();
                    }
                });
        }

        public ObservableCollection<Barcode> Barcodes { get; set; }
    }
}

using PVScan.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace PVScan.Desktop.WPF.ViewModels.Messages.Barcodes
{
    // When the variable changes
    public class HistoryPageBarcodesChanged
    {
        public ObservableCollection<Barcode> NewValue { get; set; }
    }

    // When the collection itself changes
    public class HistoryPageBarcodesCollectionChanged
    {
        public NotifyCollectionChangedEventArgs Args { get; set; }
    }
}

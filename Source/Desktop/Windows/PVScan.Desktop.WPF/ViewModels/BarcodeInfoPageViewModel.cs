using PVScan.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PVScan.Desktop.WPF.ViewModels
{
    public class BarcodeInfoPageViewModel : BaseViewModel
    {
        public BarcodeInfoPageViewModel()
        {

        }

        public Barcode SelectedBarcode { get; set; }
    }
}

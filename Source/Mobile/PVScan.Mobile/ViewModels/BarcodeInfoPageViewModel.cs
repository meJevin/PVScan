using System;
using System.Windows.Input;
using MvvmHelpers;
using PVScan.Mobile.Models;

namespace PVScan.Mobile.ViewModels
{
    public class BarcodeInfoPageViewModel : BaseViewModel
    {
        public BarcodeInfoPageViewModel()
        {
        }

        public Barcode SelectedBarcode { get; set; }

        public ICommand DeleteCommand { get; set; }
        public ICommand ShowOnMapCommand { get; set; }
        public ICommand ShowOnListCommand { get; set; }
    }
}

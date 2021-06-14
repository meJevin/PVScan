using PVScan.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace PVScan.Desktop.WPF.ViewModels
{
    public class BarcodeInfoPageViewModel : BaseViewModel
    {
        public BarcodeInfoPageViewModel()
        {
            //ShowInListCommand = new Command((object b) => 
            //{
            //    // Show in list
            //});
            //ShowOnMapCommand = new Command((object b) =>
            //{
            //    // Show on map
            //});
            //DeleteCommand = new Command((object b) =>
            //{
            //    // Delete
            //});
        }

        public Barcode SelectedBarcode { get; set; }

        //public ICommand ShowInListCommand { get; set; }
        //public ICommand ShowOnMapCommand { get; set; }
        //public ICommand DeleteCommand { get; set; }
    }
}

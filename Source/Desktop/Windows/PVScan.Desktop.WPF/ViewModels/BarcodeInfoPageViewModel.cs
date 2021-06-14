using PVScan.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace PVScan.Desktop.WPF.ViewModels
{
    public class BarcodeInfoPageViewModel : BaseViewModel
    {
        public BarcodeInfoPageViewModel()
        {
            this.PropertyChanged += BarcodeInfoPageViewModel_PropertyChanged;
        }

        private void BarcodeInfoPageViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedBarcode))
            {
                SwitchOnMapButton();
            }
        }

        private void SwitchOnMapButton()
        {
            if (SelectedBarcode == null ||
                SelectedBarcode.ScanLocation == null)
            {
                SelectedBarcodeHasLocation = false;
                return;
            }

            SelectedBarcodeHasLocation = true;
        }

        public Barcode SelectedBarcode { get; set; }
        public bool SelectedBarcodeHasLocation { get; set; } = true;
    }
}

using MvvmHelpers;
using PVScan.Mobile.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using ZXing;

namespace PVScan.Mobile.ViewModels
{
    public class ScanPageViewModel : BaseViewModel
    {
        readonly PVScanMobileDbContext _context;

        public ScanPageViewModel()
        {
            ScanCommand = new Command(async (object scanResult) =>
            {
                LastResult = (scanResult as Result);

                LastBarcodeText = LastResult.Text;
                LastBarcodeType = Enum.GetName(typeof(BarcodeFormat), LastResult.BarcodeFormat);

                CanClear = true;
                CanSave = true;

                OnPropertyChanged(nameof(LastBarcodeText));
                OnPropertyChanged(nameof(LastBarcodeType));
                OnPropertyChanged(nameof(CanClear));
                OnPropertyChanged(nameof(CanSave));
            });

            ClearCommand = new Command(async () =>
            {
                LastResult = null;

                LastBarcodeText = null;
                LastBarcodeType = null;

                CanSave = false;
                CanClear = false;

                OnPropertyChanged(nameof(LastBarcodeText));
                OnPropertyChanged(nameof(LastBarcodeType));
                OnPropertyChanged(nameof(CanClear));
                OnPropertyChanged(nameof(CanSave));
            });

            SaveCommand = new Command(async () => 
            {
                // Save to DB and clear
                ClearCommand.Execute(null);
            });
        }

        private Result LastResult;

        public ICommand ScanCommand { get; }

        public ICommand ClearCommand { get; }
        
        public ICommand SaveCommand { get; }

        public bool CanClear { get; set; }

        public bool CanSave { get; set; }

        public string LastBarcodeText { get; set; }
        public string LastBarcodeType { get; set; }
    }
}

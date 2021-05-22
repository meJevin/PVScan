using PVScan.Core.Models;
using PVScan.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Desktop.WPF.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        IBarcodesRepository BarcodesRepository;

        public MainWindowViewModel(IBarcodesRepository barcodesRepository)
        {
            BarcodesRepository = barcodesRepository;

            BarcodesRepository.Save(new Barcode()
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Text = "Niggers!",
                Favorite = true,
                ScanTime = new DateTime(2000, 1, 1),
                ScanLocation = new Coordinate()
                {
                    Latitude = 30,
                    Longitude = 50,
                },
            });
        }

        public string Message
        {
            get;
            set;
        } = "Hello!";
    }
}

using PVScan.Core.Models;
using PVScan.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PVScan.Desktop.WPF.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public MainWindowViewModel()
        {
            ToggleMapScanPages = new Command(() =>
            {
                MapScanPagesToggled?.Invoke(this, new EventArgs());
            });
        }

        public ICommand ToggleMapScanPages { get; set; }

        public event EventHandler MapScanPagesToggled;
    }
}

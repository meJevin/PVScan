using System;
using System.Collections.Generic;
using PVScan.Mobile.Models;
using PVScan.Mobile.ViewModels;

namespace PVScan.Mobile.Services.Interfaces
{
    public interface IBarcodesFilter
    {
        IEnumerable<Barcode> Filter(IEnumerable<Barcode> src, Filter filter);
        IEnumerable<Barcode> Search(IEnumerable<Barcode> src, string search);
    }
}

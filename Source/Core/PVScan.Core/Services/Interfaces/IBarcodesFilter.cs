using System;
using System.Collections.Generic;
using PVScan.Core.Models;

namespace PVScan.Core.Services.Interfaces
{
    public interface IBarcodesFilter
    {
        IEnumerable<Barcode> Filter(IEnumerable<Barcode> src, Filter filter);
        IEnumerable<Barcode> Search(IEnumerable<Barcode> src, string search);
    }
}

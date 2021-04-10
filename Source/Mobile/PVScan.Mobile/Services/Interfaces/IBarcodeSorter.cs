using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PVScan.Mobile.Models;

namespace PVScan.Mobile.Services.Interfaces
{
    public interface IBarcodeSorter
    {
        Task<IEnumerable<Barcode>> Sort(IEnumerable<Barcode> barcodes, Sorting sorting);
    }
}

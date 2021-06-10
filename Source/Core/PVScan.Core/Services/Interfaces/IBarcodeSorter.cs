using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PVScan.Core.Models;

namespace PVScan.Core.Services.Interfaces
{
    public interface IBarcodeSorter
    {
        Task<IEnumerable<Barcode>> Sort(IEnumerable<Barcode> barcodes, Sorting sorting);
    }
}

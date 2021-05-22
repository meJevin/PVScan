using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PVScan.Core.Models;
using PVScan.Core.Services.Interfaces;

namespace PVScan.Core.Services
{
    public class BarcodeSorter : IBarcodeSorter
    {
        public async Task<IEnumerable<Barcode>> Sort(IEnumerable<Barcode> barcodes, Sorting sorting)
        {
            if (sorting.Field == SortingField.Date)
            {
                if (sorting.Descending)
                {
                    barcodes = barcodes.OrderByDescending(b => b.ScanTime);
                }
                else
                {
                    barcodes = barcodes.OrderBy(b => b.ScanTime);
                }
            }
            else if (sorting.Field == SortingField.Format)
            {
                if (sorting.Descending)
                {
                    barcodes = barcodes.OrderByDescending(b => b.Format);
                }
                else
                {
                    barcodes = barcodes.OrderBy(b => b.Format);
                }
            }
            else if (sorting.Field == SortingField.Text)
            {

                if (sorting.Descending)
                {
                    barcodes = barcodes.OrderByDescending(b => b.Text);
                }
                else
                {
                    barcodes = barcodes.OrderBy(b => b.Text);
                }
            }

            return barcodes;
        }
    }
}

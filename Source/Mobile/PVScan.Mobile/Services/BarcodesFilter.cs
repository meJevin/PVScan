using System;
using System.Collections.Generic;
using System.Linq;
using PVScan.Mobile.Models;
using PVScan.Mobile.Services.Interfaces;
using PVScan.Mobile.ViewModels;

namespace PVScan.Mobile.Services
{
    public class BarcodesFilter : IBarcodesFilter
    {
        public IEnumerable<Barcode> Filter(IEnumerable<Barcode> dbBarcodes, Filter filter)
        {
            if (filter.FromDate != null && filter.ToDate != null)
            {
                dbBarcodes = dbBarcodes
                    .Where(b => b.ScanTime >= filter.FromDate)
                    .Where(b => b.ScanTime < filter.ToDate);
            }
            else if (filter.LastType != null)
            {
                DateTime to = DateTime.Today.AddDays(1);
                DateTime from = DateTime.Today;

                if (filter.LastType == LastTimeType.Day)
                {
                    from = from.AddDays(-1);
                }
                else if (filter.LastType == LastTimeType.Week)
                {
                    from = from.AddDays(-7);
                }
                else if (filter.LastType == LastTimeType.Month)
                {
                    from = from.AddMonths(-1);
                }
                else if (filter.LastType == LastTimeType.Year)
                {
                    from = from.AddYears(-1);
                }

                dbBarcodes = dbBarcodes
                    .Where(b => b.ScanTime >= from)
                    .Where(b => b.ScanTime < to);
            }

            if (filter.BarcodeFormats.Any())
            {
                dbBarcodes = dbBarcodes
                    .Where(b => filter.BarcodeFormats.Contains(b.Format));
            }

            return dbBarcodes;
        }

        public IEnumerable<Barcode> Search(IEnumerable<Barcode> barcodes, string search)
        {
            return barcodes.Where(b => b.Text.ToLower().Contains(search.ToLower()));
        }
    }
}

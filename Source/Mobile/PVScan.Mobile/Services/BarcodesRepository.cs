using PVScan.Mobile.DAL;
using PVScan.Mobile.Models;
using PVScan.Mobile.Services.Interfaces;
using PVScan.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Mobile.Services
{
    public class BarcodesRepository : IBarcodesRepository
    {
        private readonly PVScanMobileDbContext _context;

        public BarcodesRepository()
        {
            _context = Resolver.Resolve<PVScanMobileDbContext>();
        }

        public async Task Delete(Barcode barcode)
        {
            // For batch deletions..
            _context.Barcodes.Remove(barcode);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Barcode>> GetAll()
        {
            return _context.Barcodes.AsQueryable();
        }

        public async Task<IEnumerable<Barcode>> GetAllFiltered(Filter filter)
        {
            var dbBarcodes = _context.Barcodes.AsQueryable();

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

        public async Task<Barcode> Save(Barcode barcode)
        {
            _context.Barcodes.Add(barcode);
            await _context.SaveChangesAsync();

            return barcode;
        }
    }
}

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
        readonly PVScanMobileDbContext _context;

        public BarcodesRepository(PVScanMobileDbContext ctx)
        {
            _context = ctx;
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

        public async Task<Barcode> Save(Barcode barcode)
        {
            _context.Barcodes.Add(barcode);
            await _context.SaveChangesAsync();

            return barcode;
        }
    }
}

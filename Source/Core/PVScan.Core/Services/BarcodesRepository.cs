using PVScan.Core.DAL;
using PVScan.Core.Models;
using PVScan.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Core.Services
{
    public class BarcodesRepository : IBarcodesRepository
    {
        readonly PVScanDbContext _context;

        public BarcodesRepository(PVScanDbContext ctx)
        {
            _context = ctx;
        }

        public async Task Delete(Barcode barcode)
        {
            // For batch deletions..
            _context.Barcodes.Remove(barcode);

            await _context.SaveChangesAsync();
        }

        public async Task<Barcode> FindByGUID(string GUID)
        {
            var result = _context.Barcodes.Where(b => b.GUID == GUID).FirstOrDefault();

            return result;
        }

        public async Task<IEnumerable<Barcode>> GetAll()
        {
            return _context.Barcodes.AsQueryable();
        }

        public async Task<Barcode> Save(Barcode barcode)
        {
            if (string.IsNullOrEmpty(barcode.GUID))
            {
                barcode.GUID = Guid.NewGuid().ToString();
            }

            barcode.Hash = Barcode.HashOf(barcode);

            _context.Barcodes.Add(barcode);
            await _context.SaveChangesAsync();

            return barcode;
        }

        public async Task Update(Barcode barcode)
        {
            barcode.Hash = Barcode.HashOf(barcode);

            _context.Barcodes.Update(barcode);
            await _context.SaveChangesAsync();
        }
    }
}

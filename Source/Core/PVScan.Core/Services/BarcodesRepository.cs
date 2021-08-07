using Microsoft.EntityFrameworkCore;
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

        public async Task Delete(IEnumerable<Barcode> barcodes)
        {
            foreach (var b in barcodes)
            {
                _context.Barcodes.Remove(b);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<Barcode> FindByGUID(string GUID)
        {
            var result = await _context.Barcodes
                .Where(b => b.GUID == GUID)
                .FirstOrDefaultAsync();

            return result;
        }

        public async Task<IEnumerable<Barcode>> FindByGUID(IEnumerable<string> GUIDs)
        {
            var found = await _context.Barcodes
                    .Where(b => GUIDs.Contains(b.GUID))
                    .ToListAsync();

            return found;
        }

        public async Task<IEnumerable<Barcode>> GetAll()
        {
            var ret = await _context.Barcodes.ToListAsync();

            return ret;
        }

        public async Task<Barcode> Save(Barcode barcode)
        {
            SoftSave(barcode);

            await _context.SaveChangesAsync();

            return barcode;
        }

        public async Task Save(IEnumerable<Barcode> barcodes)
        {
            foreach (var b in barcodes)
            {
                SoftSave(b);
            }

            await _context.SaveChangesAsync();
        }

        public async Task Update(Barcode barcode)
        {
            SoftUpdate(barcode);

            await _context.SaveChangesAsync();
        }

        public async Task Update(IEnumerable<Barcode> barcodes)
        {
            foreach (var b in barcodes)
            {
                SoftUpdate(b);
            }

            await _context.SaveChangesAsync();
        }

        private void SoftUpdate(Barcode barcode)
        {
            barcode.LastUpdateTime = DateTime.UtcNow;

            // Precision up to milliseconds
            var lastUpdateMillisec = barcode.LastUpdateTime.Millisecond;
            var scanTimeMillisec = barcode.ScanTime.Millisecond;

            barcode.LastUpdateTime
                = barcode.LastUpdateTime.AddTicks(-(barcode.LastUpdateTime.Ticks % TimeSpan.TicksPerSecond));
            barcode.ScanTime
                = barcode.ScanTime.AddTicks(-(barcode.ScanTime.Ticks % TimeSpan.TicksPerSecond));

            barcode.LastUpdateTime
                = barcode.LastUpdateTime.Add(TimeSpan.FromMilliseconds(lastUpdateMillisec));
            barcode.ScanTime
                = barcode.ScanTime.Add(TimeSpan.FromMilliseconds(scanTimeMillisec));

            // Keep 6 decimal places on location
            if (barcode.ScanLocation != null)
            {
                barcode.ScanLocation.Latitude
                    = Math.Truncate(barcode.ScanLocation.Latitude.Value * 1e6) / 1e6;
                barcode.ScanLocation.Longitude
                    = Math.Truncate(barcode.ScanLocation.Longitude.Value * 1e6) / 1e6;
            }

            barcode.Hash = Barcode.HashOf(barcode);
            _context.Barcodes.Update(barcode);
        }

        private Barcode SoftSave(Barcode barcode)
        {
            if (string.IsNullOrEmpty(barcode.GUID))
            {
                barcode.GUID = Guid.NewGuid().ToString();
            }

            barcode.LastUpdateTime = DateTime.UtcNow;

            // Todo: these truncations probably don't belong in the repository

            // Precision up to milliseconds
            var lastUpdateMillisec = barcode.LastUpdateTime.Millisecond;
            var scanTimeMillisec = barcode.ScanTime.Millisecond;

            barcode.LastUpdateTime
                = barcode.LastUpdateTime.AddTicks(-(barcode.LastUpdateTime.Ticks % TimeSpan.TicksPerSecond));
            barcode.ScanTime
                = barcode.ScanTime.AddTicks(-(barcode.ScanTime.Ticks % TimeSpan.TicksPerSecond));

            barcode.LastUpdateTime
                = barcode.LastUpdateTime.Add(TimeSpan.FromMilliseconds(lastUpdateMillisec));
            barcode.ScanTime
                = barcode.ScanTime.Add(TimeSpan.FromMilliseconds(scanTimeMillisec));

            // Keep 6 decimal places on location
            if (barcode.ScanLocation != null)
            {
                barcode.ScanLocation.Latitude
                    = Math.Truncate(barcode.ScanLocation.Latitude.Value * 1e6) / 1e6;
                barcode.ScanLocation.Longitude
                    = Math.Truncate(barcode.ScanLocation.Longitude.Value * 1e6) / 1e6;
            }

            barcode.Hash = Barcode.HashOf(barcode);
            _context.Barcodes.Add(barcode);

            return barcode;
        }
    }
}

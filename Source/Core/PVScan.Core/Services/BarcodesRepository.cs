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
        readonly IPVScanDbContextFactory ContextFactory;

        public BarcodesRepository(IPVScanDbContextFactory fac)
        {
            ContextFactory = fac;
        }

        public async Task Delete(Barcode barcode)
        {
            await Task.Run(async () =>
            {
                using var ctx = ContextFactory.Get();
                ctx.Barcodes.Remove(barcode);

                await ctx.SaveChangesAsync();
            });
        }

        public async Task Delete(IEnumerable<Barcode> barcodes)
        {
            await Task.Run(async () =>
            {
                using var ctx = ContextFactory.Get();

                foreach (var b in barcodes)
                {
                    ctx.Barcodes.Remove(b);
                }

                await ctx.SaveChangesAsync();
            });
        }

        public async Task<Barcode> FindByGUID(string GUID)
        {
            return await Task.Run(async () =>
            {
                using var ctx = ContextFactory.Get();

                var result = await ctx.Barcodes
                    .Where(b => b.GUID == GUID)
                    .FirstOrDefaultAsync();

                return result;
            });
        }

        public async Task<IEnumerable<Barcode>> FindByGUID(IEnumerable<string> GUIDs)
        {
            return await Task.Run(async () =>
            {
                using var ctx = ContextFactory.Get();

                var found = await ctx.Barcodes
                        .Where(b => GUIDs.Contains(b.GUID))
                        .ToListAsync();

                return found;
            });
        }

        public async Task<IEnumerable<Barcode>> GetAll()
        {
            return await Task.Run(async () =>
            {
                using var ctx = ContextFactory.Get();

                return await ctx.Barcodes.ToListAsync();
            });
        }

        public async Task<Barcode> Save(Barcode barcode)
        {
            return await Task.Run(async () =>
            {
                using var ctx = ContextFactory.Get();

                SaveBarcode(barcode, ctx);

                await ctx.SaveChangesAsync();

                return barcode;
            });
        }

        public async Task Save(IEnumerable<Barcode> barcodes)
        {
            await Task.Run(async () =>
            {
                using var ctx = ContextFactory.Get();

                foreach (var b in barcodes)
                {
                    SaveBarcode(b, ctx);
                }

                await ctx.SaveChangesAsync();
            });
        }

        public async Task Update(Barcode barcode)
        {
            await Task.Run(async () =>
            {
                using var ctx = ContextFactory.Get();

                UpdateBarcode(barcode, ctx);

                await ctx.SaveChangesAsync();
            });
        }

        public async Task Update(IEnumerable<Barcode> barcodes)
        {
            await Task.Run(async () =>
            {
                using var ctx = ContextFactory.Get();

                foreach (var b in barcodes)
                {
                    UpdateBarcode(b, ctx);
                }

                await ctx.SaveChangesAsync();
            });
        }

        private void UpdateBarcode(Barcode barcode, PVScanDbContext context)
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
            context.Barcodes.Update(barcode);
        }
        private Barcode SaveBarcode(Barcode barcode, PVScanDbContext context)
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
            context.Barcodes.Add(barcode);

            return barcode;
        }
    }
}

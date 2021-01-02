using PVScan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.EntityFramework
{
    public static class PVScanDbContextSeed
    {
        public static async Task SeedDebugDataAsync(PVScanDbContext context)
        {
            if (!context.Users.Any())
            {
                context.Users.Add(new User() 
                {
                    Email = "test1@mail.com",
                    Username = "test1",
                    PasswordHash = "DEBUG",
                    IsExternal = false,
                });
                context.Users.Add(new User()
                {
                    Email = "test2@mail.com",
                    Username = "test2",
                    PasswordHash = "DEBUG",
                    IsExternal = false,
                });

                await context.SaveChangesAsync();
            }

            if (!context.Barcodes.Any())
            {
                var user1 = context.Users.Find(1);
                var user2 = context.Users.Find(2);

                context.Barcodes.Add(new Barcode()
                {
                    Format = BarcodeFormat.QR_CODE,
                    ScannedBy = user1,
                });
                context.Barcodes.Add(new Barcode()
                {
                    Format = BarcodeFormat.AZTEC,
                    ScannedBy = user1,
                });
                context.Barcodes.Add(new Barcode()
                {
                    Format = BarcodeFormat.QR_CODE,
                    ScannedBy = user1,
                });

                context.Barcodes.Add(new Barcode()
                {
                    Format = BarcodeFormat.DATA_MATRIX,
                    ScannedBy = user2,
                });
                context.Barcodes.Add(new Barcode()
                {
                    Format = BarcodeFormat.CODE_39,
                    ScannedBy = user2,
                });

                await context.SaveChangesAsync();
            }
        } 
    }
}

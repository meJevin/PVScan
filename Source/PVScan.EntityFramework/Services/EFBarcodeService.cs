using Microsoft.EntityFrameworkCore;
using PVScan.Domain.Models;
using PVScan.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.EntityFramework.Services
{
    public class EFBarcodeService : IBarcodeService
    {
        readonly PVScanDbContext context;

        public EFBarcodeService(PVScanDbContext context)
        {
            this.context = context;
        }

        public async Task<Barcode> Create(Barcode barcode)
        {
            if (barcode.ScannedBy == null)
            {
                throw new ArgumentNullException("Can not create barcode without a user!");
            }

            context.Barcodes.Add(barcode);

            await context.SaveChangesAsync();

            return barcode;
        }

        public async Task<IEnumerable<Barcode>> GetBarcodesForUser(User user)
        {
            var result = await context.Barcodes
                .Where(b => b.ScannedBy.Email == user.Email)
                .ToListAsync();
            
            return result;
        }
    }
}

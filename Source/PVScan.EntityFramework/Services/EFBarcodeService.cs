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
                throw new Exception("Can not create barcode without a user!");
            }

            var foundUser = await context.Users.FindAsync(barcode.ScannedBy.Id);
            if (foundUser == null)
            {
                throw new Exception("Can not find user with ID " + barcode.ScannedBy.Id);
            }

            context.Barcodes.Add(barcode);

            await context.SaveChangesAsync();

            return barcode;
        }

        public async Task<IEnumerable<Barcode>> GetBarcodesForUser(User user)
        {
            var result = context.Barcodes
                .Include(b => b.ScannedBy)
                .Where(b => b.ScannedBy.Email == user.Email);
            
            return result;
        }
    }
}

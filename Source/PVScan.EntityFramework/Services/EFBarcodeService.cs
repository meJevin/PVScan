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
            var foundUser = await context.Users.FindAsync(barcode.UserId);
            if (foundUser == null)
            {
                throw new Exception("Can not find user with ID " + barcode.UserId);
            }

            context.Barcodes.Add(barcode);

            await context.SaveChangesAsync();

            return barcode;
        }

        public async Task<IEnumerable<Barcode>> GetBarcodesForUser(User user)
        {
            var result = context.Barcodes.Where(b => b.UserId == user.Id);
            
            return result;
        }
    }
}

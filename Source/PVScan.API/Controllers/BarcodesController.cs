using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PVScan.API.Controllers.Base;
using PVScan.API.ViewModels.Barcodes;
using PVScan.Database;
using PVScan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PVScan.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BarcodesController : PVScanBaseController
    {
        PVScanDbContext _context;

        public BarcodesController(PVScanDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("scanned")]
        public async Task<IActionResult> Scanned(ScannedViewModel data)
        {
            var barcodeScanned = new Barcode()
            {
                Format = data.Format,
                ScanLocation = new Coordinate() { Latitude = data.Latitude, Longitude = data.Longitude },
                Text = data.Text,
                UserId = UserId,
            };

            await _context.Barcodes.AddAsync(barcodeScanned);
            await _context.SaveChangesAsync();

            return Created("", barcodeScanned);
        }
    }
}

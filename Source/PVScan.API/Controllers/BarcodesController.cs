using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PVScan.API.Controllers.Base;
using PVScan.API.Services.Interfaces;
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
    public class BarcodesController : APIBaseController
    {
        PVScanDbContext _context;
        IExperienceCalculator _expCalc;

        public BarcodesController(PVScanDbContext context, IExperienceCalculator expCalc)
        {
            _context = context;
            _expCalc = expCalc;
        }

        [HttpPost]
        [Route("scanned")]
        public async Task<IActionResult> Scanned(ScannedRequestViewModel data)
        {
            // Create barcode
            var barcodeScanned = new Barcode()
            {
                Format = data.Format,
                ScanLocation = new Coordinate() { Latitude = data.Latitude, Longitude = data.Longitude },
                Text = data.Text,
                UserId = UserId,
            };

            // Add it to DB
            await _context.Barcodes.AddAsync(barcodeScanned);
            await _context.SaveChangesAsync();

            // Calculate experience added
            var userInfo = await _context.UserInfos
                .Where(u => u.UserId == UserId)
                .FirstOrDefaultAsync();

            var experienceGained = _expCalc.GetExperienceForBarcode(userInfo);

            // Add it to user info and save
            userInfo.Experience += experienceGained;
            await _context.SaveChangesAsync();

            // Form response and send back
            ScannedResponseViewModel response = new ScannedResponseViewModel()
            {
                ExperienceGained = experienceGained,
                Barcode = barcodeScanned,
            };

            return Ok(response);
        }
    }
}

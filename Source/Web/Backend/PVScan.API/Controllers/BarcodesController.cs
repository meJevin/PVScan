using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> Scanned(ScannedRequest data)
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

            // Add it to user info and check for levelup
            userInfo.Experience += experienceGained;

            var experienceNextLevel = _expCalc.GetRequiredLevelExperience(userInfo.Level);
            double expLeft = userInfo.Experience;
            int lvlsGained = 0;
            while (expLeft >= experienceNextLevel)
            {
                // Level up
                expLeft -= experienceNextLevel;
                
                ++userInfo.Level;
                ++lvlsGained;

                experienceNextLevel = _expCalc.GetRequiredLevelExperience(userInfo.Level);
            }
            userInfo.Experience = expLeft;

            await _context.SaveChangesAsync();

            // Form response and send back
            ScannedResponse response = new ScannedResponse()
            {
                ExperienceGained = experienceGained,
                LevelsGained = lvlsGained,
                Barcode = barcodeScanned,
                UserExperience = userInfo.Experience,
                UserLevel = userInfo.Level
            };

            return Ok(response);
        }
    }
}

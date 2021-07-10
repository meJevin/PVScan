using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PVScan.API.Controllers.Base;
using PVScan.API.Hubs;
using PVScan.API.Services.Interfaces;
using PVScan.API.ViewModels.Barcodes;
using PVScan.API.ViewModels.Users;
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
        // Maybe don't use Context directly :)
        readonly IHubContext<UserInfoHub> _userInfoHub;
        readonly PVScanDbContext _context;
        readonly IExperienceCalculator _expCalc;

        public BarcodesController(PVScanDbContext context, 
            IExperienceCalculator expCalc,
            IHubContext<UserInfoHub> userInfoHub)
        {
            _context = context;
            _expCalc = expCalc;
            _userInfoHub = userInfoHub;
        }

        [HttpPost]
        [Route("scanned")]
        public async Task<IActionResult> Scanned(ScannedRequest data)
        {
            // Create barcode
            var barcodeScanned = new Barcode()
            {
                Format = data.Format,
                ScanLocation = null,
                Text = data.Text,
                UserId = UserId,
                ScanTime = data.ScanTime,
                Favorite = false,
                GUID = data.GUID,
                Hash = data.Hash,
                LastUpdateTime = data.LastTimeUpdated,
            };

            if (data.Latitude.HasValue && data.Longitude.HasValue)
            {
                barcodeScanned.ScanLocation = new Coordinate() 
                {
                    Latitude = data.Latitude.Value, 
                    Longitude = data.Longitude.Value 
                };
            }

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

            var userScannedBarcodes = await _context.Barcodes
                .Where(b => b.UserId == UserId)
                .CountAsync();

            var userScannedBarcodeFormats = await _context.Barcodes
                .Select(b => b.Format)
                .Distinct()
                .CountAsync();

            userInfo.BarcodesScanned = userScannedBarcodes;
            userInfo.BarcodeFormatsScanned = userScannedBarcodeFormats;

            _context.UserInfos.Update(userInfo);
            await _context.SaveChangesAsync();

            if (_userInfoHub != null)
            {
                await _userInfoHub.Clients
                    .Groups(User.FindFirstValue(ClaimTypes.NameIdentifier))
                    .SendAsync("Changed", new CurrentResponse()
                    {
                        BarcodeFormatsScanned = userInfo.BarcodeFormatsScanned,
                        BarcodesScanned = userInfo.BarcodesScanned,
                        Experience = userInfo.Experience,
                        IGLink = userInfo.IGLink,
                        Level = userInfo.Level,
                        VKLink = userInfo.VKLink,
                    });
            }

            // Form response and send back
            ScannedResponse response = new ScannedResponse()
            {
                ExperienceGained = experienceGained,
                LevelsGained = lvlsGained,
                UserExperience = userInfo.Experience,
                UserLevel = userInfo.Level,
                UserBarcodesScanned = userScannedBarcodes,
                UserBarcodeFormatsScanned = userScannedBarcodeFormats,
            };

            return Ok(response);
        }

        [HttpPost]
        [Route("updated")]
        public async Task<IActionResult> Updated(UpdatedRequest data)
        {
            var barcodeFromDB = _context.Barcodes
                .Where(b => b.UserId == UserId && b.GUID == data.GUID)
                .FirstOrDefault();

            if (barcodeFromDB == null)
            {
                return NotFound();
            }

            if (data.Latitude.HasValue && data.Longitude.HasValue)
            {
                barcodeFromDB.ScanLocation = new Coordinate()
                {
                    Latitude = data.Latitude.Value,
                    Longitude = data.Longitude.Value
                };
            }

            barcodeFromDB.Favorite = data.Favorite;
            barcodeFromDB.LastUpdateTime = data.LastTimeUpdated;
            barcodeFromDB.Hash = Barcode.HashOf(barcodeFromDB);

            _context.Barcodes.Update(barcodeFromDB);
            await _context.SaveChangesAsync();

            return Ok(new UpdatedResponse() { });
        }

        [HttpPost]
        [Route("deleted")]
        public async Task<IActionResult> Deleted(DeletedRequest data)
        {
            var userInfo = await _context.UserInfos
                .Where(u => u.UserId == UserId)
                .FirstOrDefaultAsync();

            var barcodeFromDB = _context.Barcodes
                .Where(b => b.UserId == UserId && b.GUID == data.GUID)
                .FirstOrDefault();

            if (barcodeFromDB == null || userInfo == null)
            {
                return NotFound();
            }

            _context.Barcodes.Remove(barcodeFromDB);
            await _context.SaveChangesAsync();

            var userScannedBarcodes = await _context.Barcodes
                .Where(b => b.UserId == UserId)
                .CountAsync();

            var userScannedBarcodeFormats = await _context.Barcodes
                .Select(b => b.Format)
                .Distinct()
                .CountAsync();

            userInfo.BarcodesScanned = userScannedBarcodes;
            userInfo.BarcodeFormatsScanned = userScannedBarcodeFormats;

            _context.UserInfos.Update(userInfo);
            await _context.SaveChangesAsync();

            if (_userInfoHub != null)
            {
                await _userInfoHub.Clients
                    .Groups(User.FindFirstValue(ClaimTypes.NameIdentifier))
                    .SendAsync("Changed", new CurrentResponse()
                    {
                        BarcodeFormatsScanned = userInfo.BarcodeFormatsScanned,
                        BarcodesScanned = userInfo.BarcodesScanned,
                        Experience = userInfo.Experience,
                        IGLink = userInfo.IGLink,
                        Level = userInfo.Level,
                        VKLink = userInfo.VKLink,
                    });
            }

            return Ok(new DeletedResponse() { });
        }
    }
}

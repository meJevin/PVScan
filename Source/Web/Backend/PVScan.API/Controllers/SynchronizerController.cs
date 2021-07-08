using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PVScan.API.Controllers.Base;
using PVScan.API.Hubs;
using PVScan.API.Services.Interfaces;
using PVScan.API.ViewModels.Barcodes;
using PVScan.API.ViewModels.Synchronizer;
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
    public class SynchronizerController : APIBaseController
    {
        // Maybe don't use Context directly :)
        readonly PVScanDbContext _context;

        public SynchronizerController(PVScanDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("synchronize")]
        public async Task<IActionResult> Synchronize(SynchronizeRequest req)
        {
            // Todo: This will be really slow with large amounts of barcodes
            var serverBarcodes = _context.Barcodes.ToList();

            var toUpdateLocaly = serverBarcodes
                .Where(b => req.LocalBarcodeInfos
                    .Any(item => item.GUID == b.GUID && item.Hash != b.Hash && item.LastTimeUpdated < b.LastUpdateTime));

            var toAddLocaly = serverBarcodes
                .Where(b => !req.LocalBarcodeInfos
                    .Any(item => item.GUID == b.GUID));

            var toAddToServer = req.LocalBarcodeInfos.Select(r => r.GUID).Except(serverBarcodes.Select(b => b.GUID));

            var toUpdateOnServer = serverBarcodes
                .Where(b => req.LocalBarcodeInfos
                    .Any(item => item.GUID == b.GUID && item.Hash != b.Hash && item.LastTimeUpdated >= b.LastUpdateTime))
                .Select(b => b.GUID);

            foreach (var updateLocaly in toUpdateLocaly)
            {
                updateLocaly.Id = req.LocalBarcodeInfos.Where(i => i.GUID == updateLocaly.GUID).First().LocalId;
            }

            foreach (var addLocaly in toAddLocaly)
            {
                addLocaly.Id = 0;
            }

            return Ok(new SynchronizeResponse()
            {
                ToAddLocaly = toAddLocaly,
                ToUpdateLocaly = toUpdateLocaly,
                ToAddToServer = toAddToServer,
                ToUpdateOnServer = toUpdateOnServer,
            });
        }
    }
}

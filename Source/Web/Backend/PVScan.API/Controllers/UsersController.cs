using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PVScan.API.Controllers.Base;
using PVScan.API.Hubs;
using PVScan.Domain.DTO.Users;
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
    public class UsersController : APIBaseController
    {
        readonly PVScanDbContext _context;
        readonly IHubContext<UserInfoHub> _userInfoHub;

        public UsersController(PVScanDbContext context, 
            IHubContext<UserInfoHub> userInfoHub)
        {
            _context = context;
            _userInfoHub = userInfoHub;
        }

        [HttpGet]
        [Route("current")]
        public async Task<IActionResult> Current()
        {
            var userInfo = await _context.UserInfos
                .Where(u => u.UserId == UserId)
                .FirstOrDefaultAsync();

            var aspUser = await _context.Users
                .Where(u => u.Id == UserId)
                .FirstOrDefaultAsync();

            if (userInfo == null || aspUser == null)
            {
                return NotFound();
            }

            return Ok(new CurrentResponse() 
            {
                BarcodeFormatsScanned = userInfo.BarcodeFormatsScanned,
                BarcodesScanned = userInfo.BarcodesScanned,
                Experience = userInfo.Experience,
                IGLink = userInfo.IGLink,
                Level = userInfo.Level,
                VKLink = userInfo.VKLink,
                Email = aspUser.Email,
                Username = aspUser.UserName,
            });
        }

        [HttpPost]
        [Route("change")]
        public async Task<IActionResult> Change(ChangeRequest data)
        {
            var userInfo = await _context.UserInfos
                .Where(u => u.UserId == UserId)
                .FirstOrDefaultAsync();

            if (userInfo == null)
            {
                return NotFound();
            }

            userInfo.IGLink = data.IGLink;
            userInfo.VKLink = data.VKLink;

            await _context.SaveChangesAsync();

            return Ok(new ChangeResponse()
            {
                BarcodeFormatsScanned = userInfo.BarcodeFormatsScanned,
                BarcodesScanned = userInfo.BarcodesScanned,
                Experience = userInfo.Experience,
                IGLink = userInfo.IGLink,
                Level = userInfo.Level,
                VKLink = userInfo.VKLink,
            });
        }
    }
}

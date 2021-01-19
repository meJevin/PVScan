using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PVScan.API.Controllers.Base;
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
    public class UsersController : APIBaseController
    {
        PVScanDbContext _context;

        public UsersController(PVScanDbContext context)
        {
            _context = context;
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

            return Ok(new CurrentResponseViewModel() 
            {
                BarcodeFormatsScanned = userInfo.BarcodeFormatsScanned,
                BarcodesScanned = userInfo.BarcodesScanned,
                Experience = userInfo.Experience,
                Level = userInfo.Level,
                IGLink = userInfo.IGLink,
                VKLink = userInfo.VKLink,
                Email = aspUser.Email,
                Username = aspUser.UserName,
            });
        }

        [HttpPost]
        [Route("change")]
        public async Task<IActionResult> Change(ChangeRequestViewModel data)
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

            return Ok();
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class UsersController : ControllerBase
    {
        PVScanDbContext _context;

        public UsersController(PVScanDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("info")]
        public async Task<IActionResult> Info(InfoViewModel data)
        {
            var userInfo = await _context.UserInfos
                .Where(u => u.UserId == data.UserId)
                .FirstOrDefaultAsync();

            if (userInfo == null)
            {
                return NotFound();
            }

            return Ok(userInfo);
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using PVScan.Database;
using PVScan.Database.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PVScan.Auth.Tests
{
    public class TestBase
    {
        readonly protected PVScanDbContext _context;
        readonly static List<ApplicationUser> _fakeUsers = new List<ApplicationUser>()
        {
            new ApplicationUser()
            {
                UserName = "test1",
                Email = "test1@mail.com",
            }
        };

        public TestBase()
        {
            var options = new DbContextOptionsBuilder<PVScanDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new PVScanDbContext(options);
        }
    }
}

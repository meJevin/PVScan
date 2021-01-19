using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PVScan.Database;
using PVScan.Database.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.API.Tests
{
    public class TestBase
    {
        readonly protected PVScanDbContext _context;
        readonly protected ControllerContext _mockControlerContext;

        readonly protected string MockUserName = "testUserName";
        readonly protected string MockUserId = "testUserId";

        public TestBase()
        {
            var options = new DbContextOptionsBuilder<PVScanDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new PVScanDbContext(options);

            _mockControlerContext = new ControllerContext();
            _mockControlerContext.HttpContext = new DefaultHttpContext();
            _mockControlerContext.HttpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, MockUserName),
                    new Claim(ClaimTypes.NameIdentifier, MockUserId),
                }, "mock"));

            _context.Users.Add(new ApplicationUser() { Id = MockUserId });
            _context.SaveChanges();
        }

        public void SeedDatabase()
        {
        }
    }
}

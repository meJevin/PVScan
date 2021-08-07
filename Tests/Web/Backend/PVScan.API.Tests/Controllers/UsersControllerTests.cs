using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PVScan.API.Controllers;
using PVScan.Domain.DTO.Users;
using PVScan.Domain.Entities;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PVScan.API.Tests.Controllers
{
    public class UsersControllerTests : TestBase
    {
        [Fact]
        public async Task Can_Get_Current_User_Info()
        {
            // Arrange
            UsersController controller = new UsersController(_context, null);
            controller.ControllerContext = _mockControlerContext;

            _context.UserInfos.Add(new UserInfo()
            {
                UserId = MockUserId,
                BarcodesScanned = 10,
            });
            _context.SaveChanges();

            // Act
            var result = (await controller.Current()) as ObjectResult;
            var resultObject = result.Value as CurrentResponse;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, resultObject.BarcodesScanned);
        }

        [Fact]
        public async Task Can_Change_Current_User_Info()
        {
            // Arrange
            UsersController controller = new UsersController(_context, null);
            controller.ControllerContext = _mockControlerContext;

            _context.UserInfos.Add(new UserInfo()
            {
                UserId = MockUserId,
            });
            _context.SaveChanges();

            // Act
            var result = (await controller.Change(new ChangeRequest() { IGLink = "test" }) as ObjectResult).Value as ChangeResponse;
            var resultObject = await _context.UserInfos.FirstOrDefaultAsync(u => u.UserId == MockUserId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test", resultObject.IGLink);
        }
    }
}

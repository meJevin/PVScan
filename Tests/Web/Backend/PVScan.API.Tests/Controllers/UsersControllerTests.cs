using Microsoft.AspNetCore.Mvc;
using PVScan.API.Controllers;
using PVScan.API.ViewModels.Users;
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
            UsersController controller = new UsersController(_context);
            controller.ControllerContext = _mockControlerContext;

            _context.UserInfos.Add(new UserInfo()
            {
                UserId = MockUserId,
                BarcodesScanned = 10,
            });
            _context.SaveChanges();

            // Act
            var result = (await controller.Info(new GetInfoViewModel() { }) as ObjectResult);
            var resultObject = result.Value as UserInfo;

            // Assert
            Assert.NotNull(resultObject);
            Assert.Equal(10, resultObject.BarcodesScanned);
        }

        [Fact]
        public async Task Can_Change_Current_User_Info()
        {
            // Arrange
            UsersController controller = new UsersController(_context);
            controller.ControllerContext = _mockControlerContext;

            _context.UserInfos.Add(new UserInfo()
            {
                UserId = MockUserId,
            });
            _context.SaveChanges();

            // Act
            var result = (await controller.Info(new ChangeInfoViewModel() { IGLink = "test" }) as ObjectResult);
            var resultObject = result.Value as UserInfo;

            // Assert
            Assert.NotNull(resultObject);
            Assert.Equal("test", resultObject.IGLink);
        }
    }
}

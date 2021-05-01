using Microsoft.AspNetCore.Mvc;
using Moq;
using PVScan.API.Controllers;
using PVScan.API.Services;
using PVScan.API.Services.Interfaces;
using PVScan.API.ViewModels.Barcodes;
using PVScan.Domain.Entities;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PVScan.API.Tests.Controllers
{
    public class BarcodesControllerTests : TestBase
    {
        [Fact]
        public async Task Can_Send_Scanned_Barcode()
        {
            // Arrange
            BarcodesController controller = new BarcodesController(_context, new ExperienceCalculator(_context));
            controller.ControllerContext = _mockControlerContext;

            _context.UserInfos.Add(new UserInfo()
            {
                UserId = MockUserId,
                BarcodesScanned = 10,
            });
            _context.SaveChanges();

            // Act
            var result = (await controller.Scanned(new ScannedRequest()
            {
                Format = Domain.Enums.BarcodeFormat.QR_CODE,
                Latitude = 30,
                Longitude = 50,
                Text = "test",
            }) as ObjectResult);
            var resultObject = result.Value as ScannedResponse;

            // Assert
            Assert.NotNull(resultObject);
            Assert.Equal(30, resultObject.Barcode.ScanLocation.Latitude);
            Assert.Equal(50, resultObject.Barcode.ScanLocation.Longitude);
            Assert.Equal("test", resultObject.Barcode.Text);
            Assert.Equal(Domain.Enums.BarcodeFormat.QR_CODE, resultObject.Barcode.Format);
        }

        [Fact]
        public async Task Can_Gain_Experience_From_Scanned_Barcode()
        {
            // Arrange
            BarcodesController controller = new BarcodesController(_context, new ExperienceCalculator(_context));
            controller.ControllerContext = _mockControlerContext;

            var userInfo = new UserInfo()
            {
                UserId = MockUserId,
                Experience = 0,
                Level = 1,
            };

            _context.UserInfos.Add(userInfo);
            _context.SaveChanges();

            // Act
            var result = (await controller.Scanned(new ScannedRequest(){ }) as ObjectResult);
            var resultObject = result.Value as ScannedResponse;

            // Assert
            Assert.NotNull(resultObject);
            Assert.True(resultObject.ExperienceGained > 0);
            Assert.Equal(userInfo.Experience, resultObject.ExperienceGained);
        }

        [Fact]
        public async Task Can_Gain_Level_From_Scanned_Barcode()
        {
            // Arrange
            var mockCalc = new Mock<IExperienceCalculator>();
            mockCalc.Setup(c => c.GetExperienceForBarcode(It.IsAny<UserInfo>())).Returns(15);
            mockCalc.Setup(c => c.GetRequiredLevelExperience(It.IsAny<int>())).Returns(10);

            BarcodesController controller = new BarcodesController(_context, mockCalc.Object);
            controller.ControllerContext = _mockControlerContext;

            var userInfo = new UserInfo()
            {
                UserId = MockUserId,
                Level = 1,
                Experience = 0,
            };

            _context.UserInfos.Add(userInfo);
            _context.SaveChanges();

            // Act
            var result = (await controller.Scanned(new ScannedRequest() { }) as ObjectResult);
            var resultObject = result.Value as ScannedResponse;

            
            // Assert
            Assert.NotNull(resultObject);
            Assert.Equal(1, resultObject.LevelsGained);
            Assert.Equal(2, userInfo.Level);
        }

        [Fact]
        public async Task Can_Gain_Multiple_Levels_From_Scanned_Barcode()
        {
            // Arrange
            var mockCalc = new Mock<IExperienceCalculator>();
            mockCalc.Setup(c => c.GetExperienceForBarcode(It.IsAny<UserInfo>())).Returns(57);
            mockCalc.Setup(c => c.GetRequiredLevelExperience(It.IsAny<int>())).Returns(10);

            BarcodesController controller = new BarcodesController(_context, mockCalc.Object);
            controller.ControllerContext = _mockControlerContext;

            var userInfo = new UserInfo()
            {
                UserId = MockUserId,
                Level = 1,
                Experience = 0,
            };

            _context.UserInfos.Add(userInfo);
            _context.SaveChanges();

            // Act
            var result = (await controller.Scanned(new ScannedRequest() { }) as ObjectResult);
            var resultObject = result.Value as ScannedResponse;

            // Assert
            Assert.NotNull(resultObject);
            Assert.Equal(5, resultObject.LevelsGained);
            Assert.Equal(6, userInfo.Level);
        }

        [Fact]
        public async Task After_Levels_Are_Gained_Leftover_Experience_Calculated()
        {
            // Arrange
            var mockCalc = new Mock<IExperienceCalculator>();
            mockCalc.Setup(c => c.GetExperienceForBarcode(It.IsAny<UserInfo>())).Returns(57);
            mockCalc.Setup(c => c.GetRequiredLevelExperience(It.IsAny<int>())).Returns(10);

            BarcodesController controller = new BarcodesController(_context, mockCalc.Object);
            controller.ControllerContext = _mockControlerContext;

            var userInfo = new UserInfo()
            {
                UserId = MockUserId,
                Level = 1,
                Experience = 0,
            };

            _context.UserInfos.Add(userInfo);
            _context.SaveChanges();

            // Act
            var result = (await controller.Scanned(new ScannedRequest() { }) as ObjectResult);
            var resultObject = result.Value as ScannedResponse;

            // Assert
            Assert.NotNull(resultObject);
            Assert.Equal(7, resultObject.UserExperience);
            Assert.Equal(7, userInfo.Experience);
        }
    }
}

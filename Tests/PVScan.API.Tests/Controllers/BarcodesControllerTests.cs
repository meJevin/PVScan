using Microsoft.AspNetCore.Mvc;
using PVScan.API.Controllers;
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
            BarcodesController controller = new BarcodesController(_context);
            controller.ControllerContext = _mockControlerContext;

            _context.UserInfos.Add(new UserInfo()
            {
                UserId = MockUserId,
                BarcodesScanned = 10,
            });
            _context.SaveChanges();

            // Act
            var result = (await controller.Scanned(new ScannedViewModel()
            {
                Format = Domain.Enums.BarcodeFormat.QR_CODE,
                Latitude = 30,
                Longitude = 50,
                Text = "test",
            }) as ObjectResult);
            var resultObject = result.Value as Barcode;

            // Assert
            Assert.NotNull(resultObject);
            Assert.Equal(30, resultObject.ScanLocation.Latitude);
            Assert.Equal(50, resultObject.ScanLocation.Longitude);
            Assert.Equal("test", resultObject.Text);
            Assert.Equal(Domain.Enums.BarcodeFormat.QR_CODE, resultObject.Format);
        }
    }
}

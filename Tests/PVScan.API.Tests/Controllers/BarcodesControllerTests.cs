﻿using Microsoft.AspNetCore.Mvc;
using Moq;
using PVScan.API.Controllers;
using PVScan.API.Services;
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
            var result = (await controller.Scanned(new ScannedRequestViewModel()
            {
                Format = Domain.Enums.BarcodeFormat.QR_CODE,
                Latitude = 30,
                Longitude = 50,
                Text = "test",
            }) as ObjectResult);
            var resultObject = result.Value as ScannedResponseViewModel;

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
            var result = (await controller.Scanned(new ScannedRequestViewModel(){ }) as ObjectResult);
            var resultObject = result.Value as ScannedResponseViewModel;

            // Assert
            Assert.NotNull(resultObject);
            Assert.True(resultObject.ExperienceGained > 0);
            Assert.Equal(userInfo.Experience, resultObject.ExperienceGained);
        }
    }
}

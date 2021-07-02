using Microsoft.EntityFrameworkCore;
using Moq;
using PVScan.Core.DAL;
using PVScan.Core.Models;
using PVScan.Core.Models.API;
using PVScan.Core.Services.Interfaces;
using PVScan.Mobile.Services;
using PVScan.Mobile.Services.Interfaces;
using PVScan.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PVScan.Mobile.Tests.ViewModels
{
    public class LoggedInPageViewModelTests : TestBase
    {
        Mock<IIdentityService> mockIdentityService;
        Mock<IPVScanAPI> mockAPI;
        Mock<IAPIBarcodeHub> mockBarcodeHub;
        Mock<IAPIUserInfoHub> mockUserInfoHub;

        public LoggedInPageViewModelTests()
        {
            mockIdentityService = new Mock<IIdentityService>();
            mockAPI = new Mock<IPVScanAPI>();
            mockBarcodeHub = new Mock<IAPIBarcodeHub>();
            mockUserInfoHub = new Mock<IAPIUserInfoHub>();
        }

        [Fact]
        public void Can_Logout()
        {
            // Arrange
            mockIdentityService.Setup(i => i.LogoutAsync())
                .ReturnsAsync(true);

            var sut = new LoggedInPageViewModel(
                mockIdentityService.Object,
                mockAPI.Object,
                mockBarcodeHub.Object,
                mockUserInfoHub.Object);

            bool loggedOut = false;
            sut.SuccessfulLogout += (_, _) => { loggedOut = true; };

            // Act
            sut.LogoutCommand.Execute(null);

            // Assert
            Assert.True(loggedOut);
        }

        [Fact]
        public void Can_Change_User_Info()
        {
            // Arrange
            mockAPI.Setup(i => i.ChangeUserInfo(It.IsAny<ChangeUserInfoRequest>()))
                .ReturnsAsync(new ChangeUserInfoResponse()
                {
                    BarcodeFormatsScanned = 20,
                    BarcodesScanned = 200,
                    Experience = 999,
                    IGLink = "IgLink",
                    Level = 400,
                    VKLink = "VkLink",
                });

            var sut = new LoggedInPageViewModel(
                mockIdentityService.Object,
                mockAPI.Object,
                mockBarcodeHub.Object,
                mockUserInfoHub.Object);

            // Act
            sut.SaveProfileCommand.Execute(null);

            // Assert
            Assert.Equal(20, sut.UserInfo.BarcodeFormatsScanned);
            Assert.Equal(200, sut.UserInfo.BarcodesScanned);
            Assert.Equal(900, sut.UserInfo.Experience);
            Assert.Equal("IgLink", sut.UserInfo.IGLink);
            Assert.Equal("VkLink", sut.UserInfo.VKLink);
            Assert.Equal(400, sut.UserInfo.Level);
        }
    }
}

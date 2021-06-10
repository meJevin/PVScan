using Microsoft.EntityFrameworkCore;
using Moq;
using PVScan.Core.DAL;
using PVScan.Core.Models;
using PVScan.Core.Services;
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
    public class LoginViewModelTests : TestBase
    {
        readonly Mock<IIdentityService> mockIdentityService;

        readonly string ValidLogin = "Username1";
        readonly string ValidPassword = "Password1";

        public LoginViewModelTests()
        {
            mockIdentityService = new Mock<IIdentityService>();

            mockIdentityService
                .Setup(i => i.LoginAsync(
                    It.Is<string>(s => s == ValidLogin),
                    It.Is<string>(s => s == ValidPassword)))
                .ReturnsAsync(true);

            mockIdentityService
                .Setup(i => i.LoginAsync(
                    It.Is<string>(s => s != ValidLogin),
                    It.Is<string>(s => s != ValidPassword)))
                .ReturnsAsync(false);
        }

        [Fact]
        public void Can_Login_With_Valid_Credentials()
        {
            // Arrange
            LoginPageViewModel vm = new LoginPageViewModel(
                mockIdentityService.Object,
                new Mock<IPopupMessageService>().Object,
                new Mock<IAPIBarcodeHub>().Object,
                new Mock<IAPIUserInfoHub>().Object);
            bool loggedIn = false;

            vm.Login = ValidLogin;
            vm.Password = ValidPassword;

            vm.SuccessfulLogin += (s, a) => { loggedIn = true; };

            // Act
            vm.LoginCommand.Execute(null);

            // Assert
            mockIdentityService.Verify(m => m.LoginAsync(ValidLogin, ValidPassword), Times.Once);
            Assert.True(loggedIn);
        }

        [Theory]
        [InlineData("Somethin", "Something")]
        [InlineData("Somethin1231", "Something5436")]
        [InlineData("Somet657567hin", "Someth53456ing")]
        public void Can_Not_Login_With_Invalid_Credentials(string login, string password)
        {
            // Arrange
            LoginPageViewModel vm = new LoginPageViewModel(
                mockIdentityService.Object,
                new Mock<IPopupMessageService>().Object,
                new Mock<IAPIBarcodeHub>().Object,
                new Mock<IAPIUserInfoHub>().Object);
            bool loggedIn = true;

            vm.Login = login;
            vm.Password = password;

            vm.FailedLogin += (s, a) => { loggedIn = false; };

            // Act
            vm.LoginCommand.Execute(null);

            // Assert
            mockIdentityService.Verify(m => m.LoginAsync(login, password), Times.Once);
            Assert.False(loggedIn);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData(null, "")]
        [InlineData("", null)]
        [InlineData(null, null)]
        public void Login_With_Empty_Credentials_Authentication_Not_Requested(string login, string password)
        {
            // Arrange
            LoginPageViewModel vm = new LoginPageViewModel(
                mockIdentityService.Object,
                new Mock<IPopupMessageService>().Object,
                new Mock<IAPIBarcodeHub>().Object,
                new Mock<IAPIUserInfoHub>().Object);

            vm.Login = login;
            vm.Password = password;

            // Act
            vm.LoginCommand.Execute(null);

            // Assert
            mockIdentityService.Verify(m => m.LoginAsync(login, password), Times.Never);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Moq;
using PVScan.Mobile.DAL;
using PVScan.Mobile.Models;
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
    // Todo: add existing user check
    public class SignUpPageViewModelTests : TestBase
    {
        readonly Mock<IIdentityService> mockIdentityService;

        public SignUpPageViewModelTests()
        {
            mockIdentityService = new Mock<IIdentityService>();

            mockIdentityService
                .Setup(i => i.SignUpAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(true);
        }

        [Theory]
        [InlineData("Somethin", "Something", "test@mail.com")]
        [InlineData("Somethin1231", "Something5436", "test1@mail.com")]
        [InlineData("Somet657567hin", "Someth53456ing", "test2@mail.com")]
        public void Can_SignUp_With_Valid_Credentials(string login, string password, string email)
        {
            // Arrange
            SignUpPageViewModel vm = new SignUpPageViewModel(mockIdentityService.Object);
            bool signedUp = false;

            vm.Login = login;
            vm.Password = password;
            vm.Email = email;

            vm.SuccessfulSignUp += (s, a) => { signedUp = true; };

            // Act
            vm.SignUpCommand.Execute(null);

            // Assert
            mockIdentityService.Verify(m => m.SignUpAsync(login, password, email), Times.Once);
            Assert.True(signedUp);
        }

        [Theory]
        [InlineData("", "", "")]
        [InlineData(null, "", "")]
        [InlineData("", null, "")]
        [InlineData(null, null, "")]
        [InlineData("", "", null)]
        [InlineData(null, "", null)]
        [InlineData("", null, null)]
        [InlineData(null, null, null)]
        public void SignUp_With_Empty_Credentials_SignUp_Not_Requested(string login, string password, string email)
        {
            // Arrange
            SignUpPageViewModel vm = new SignUpPageViewModel(mockIdentityService.Object);

            vm.Login = login;
            vm.Password = password;
            vm.Email = email;

            // Act
            vm.SignUpCommand.Execute(null);

            // Assert
            mockIdentityService.Verify(m => m.LoginAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}

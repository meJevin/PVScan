using Microsoft.EntityFrameworkCore;
using Moq;
using PVScan.Core;
using PVScan.Core.DAL;
using PVScan.Core.Models;
using PVScan.Core.Services.Interfaces;
using PVScan.Mobile.Services;
using PVScan.Mobile.Services.Interfaces;
using PVScan.Mobile.ViewModels;
using Xunit;
using PVScan.Core.Services.Mocks;

namespace PVScan.Mobile.Tests.ViewModels
{
    public class ApplicationSettingsPageViewModelTests : TestBase
    {
        IPersistentKVP KVP;

        public ApplicationSettingsPageViewModelTests()
        {
            KVP = new InMemoryKVP();
        }

        [Fact]
        public void Theme_Set_To_Light_Displayed_In_Settings()
        {
            // Arrange
            Xamarin.Forms.Application.Current.UserAppTheme = Xamarin.Forms.OSAppTheme.Light;

            // Act
            ApplicationSettingsPageViewModel vm = new ApplicationSettingsPageViewModel(KVP);

            // Assert
            Assert.False(vm.IsDarkTheme);
        }

        [Fact]
        public void Theme_Set_To_Dark_Displayed_In_Settings()
        {
            // Arrange
            Xamarin.Forms.Application.Current.UserAppTheme = Xamarin.Forms.OSAppTheme.Dark;

            // Act
            ApplicationSettingsPageViewModel vm = new ApplicationSettingsPageViewModel(KVP);

            // Assert
            Assert.True(vm.IsDarkTheme);
        }

        [Theory]
        [InlineData("Dark", "Light")]
        [InlineData("Light", "Dark")]
        public void Theme_Toggled_Changes_Current_Theme(string oldTheme, string newTheme)
        {
            // Arrange
            KVP.Clear();
            KVP.Set(StorageKeys.Theme, oldTheme);
            ApplicationSettingsPageViewModel vm = new ApplicationSettingsPageViewModel(KVP);

            // Act
            vm.IsDarkTheme = newTheme == "Dark";
            vm.SwitchThemeCommand.Execute(null);

            // Assert
            Assert.Equal(newTheme, KVP.Get(StorageKeys.Theme, null));
        }
    }
}

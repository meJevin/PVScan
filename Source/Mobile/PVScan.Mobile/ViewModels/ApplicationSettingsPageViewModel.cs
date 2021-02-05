using MvvmHelpers;
using PVScan.Mobile.ViewModels.Messages.Scanning;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PVScan.Mobile.ViewModels
{
    public class ApplicationSettingsPageViewModel : BaseViewModel
    {
        public ApplicationSettingsPageViewModel()
        {
            SwitchThemeCommand = new Command(async () =>
            {
                if (IsDarkTheme)
                {
                    Application.Current.UserAppTheme = OSAppTheme.Dark;

                    Preferences.Set("Theme", "Dark");
                }
                else
                {
                    Application.Current.UserAppTheme = OSAppTheme.Light;
                 
                    Preferences.Set("Theme", "Light");
                }
            });

            AllowCameraCommand = new Command(async () =>
            {
                var result = await Permissions.RequestAsync<Permissions.Camera>();

                if (result == PermissionStatus.Granted)
                {
                    MessagingCenter.Send(this, nameof(CameraAllowedMessage), new CameraAllowedMessage());
                    CameraToggleEnabled = false;
                }
            });

            CameraToggleEnabled = !(Permissions.CheckStatusAsync<Permissions.Camera>()
                .GetAwaiter().GetResult() == PermissionStatus.Granted);

            IsDarkTheme = Application.Current.UserAppTheme == OSAppTheme.Dark;
        }

        public ICommand SwitchThemeCommand { get; }
        public ICommand AllowCameraCommand { get; }

        public bool IsDarkTheme { get; set; }
        public bool CameraToggleEnabled { get; set; }
    }
}

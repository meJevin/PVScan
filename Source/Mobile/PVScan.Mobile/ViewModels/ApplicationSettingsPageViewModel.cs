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

            IsDarkTheme = Application.Current.UserAppTheme == OSAppTheme.Dark;
        }

        public ICommand SwitchThemeCommand { get; }

        public bool IsDarkTheme { get; set; }
    }
}

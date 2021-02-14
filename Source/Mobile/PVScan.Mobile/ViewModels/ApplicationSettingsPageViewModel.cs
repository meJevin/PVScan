﻿using MvvmHelpers;
using PVScan.Mobile.Services.Interfaces;
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
        readonly IPersistentKVP KVP;

        public ApplicationSettingsPageViewModel(IPersistentKVP kvp)
        {
            KVP = kvp;

            SwitchThemeCommand = new Command(async () =>
            {
                if (IsDarkTheme)
                {
                    Application.Current.UserAppTheme = OSAppTheme.Dark;

                    KVP.Set("Theme", "Dark");
                }
                else
                {
                    Application.Current.UserAppTheme = OSAppTheme.Light;

                    KVP.Set("Theme", "Light");
                }
            });

            IsDarkTheme = Application.Current.UserAppTheme == OSAppTheme.Dark;
        }

        public ICommand SwitchThemeCommand { get; }

        public bool IsDarkTheme { get; set; }
    }
}

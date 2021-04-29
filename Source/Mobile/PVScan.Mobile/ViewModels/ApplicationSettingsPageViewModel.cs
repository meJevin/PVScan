using MvvmHelpers;
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
                    KVP.Set(StorageKeys.Theme, "Dark");
                }
                else
                {
                    Application.Current.UserAppTheme = OSAppTheme.Light;
                    KVP.Set(StorageKeys.Theme, "Light");
                }
            });

            SwitchSaveBarcodeImagesWithAlphaCommand = new Command(async () =>
            {
                if (SaveBarcodeImagesWithAlpha)
                {
                    KVP.Set(StorageKeys.SaveBarcodeImagesWithAlpha, true);
                }
                else
                {
                    KVP.Set(StorageKeys.SaveBarcodeImagesWithAlpha, false);
                }
            });

            IsDarkTheme = Application.Current.UserAppTheme == OSAppTheme.Dark;
            SaveBarcodeImagesWithAlpha = KVP.Get(StorageKeys.SaveBarcodeImagesWithAlpha, StorageKeys.Defaults.SaveBarcodeImagesWithAlpha);
        }

        public ICommand SwitchThemeCommand { get; }
        public ICommand SwitchSaveBarcodeImagesWithAlphaCommand { get; }

        public bool IsDarkTheme { get; set; }
        public bool SaveBarcodeImagesWithAlpha { get; set; }
    }
}

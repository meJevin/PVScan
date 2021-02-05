using PVScan.Mobile.Services.Identity;
using PVScan.Mobile.Views;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace PVScan.Mobile
{
    public partial class App : Xamarin.Forms.Application
    {

        public App()
        {
            var preferencesTheme = Preferences.Get("Theme", "N/A");

            if (preferencesTheme == "N/A")
            {
                UserAppTheme = preferencesTheme == "Light" ? OSAppTheme.Light : OSAppTheme.Dark;
            }
            else
            {
                UserAppTheme = Xamarin.Forms.Application.Current.RequestedTheme;
            }

            InitializeComponent();

            var navigationPage = new Xamarin.Forms.NavigationPage(new MainPage());
            navigationPage.On<iOS>().SetHideNavigationBarSeparator(false);
            navigationPage.SetOnAppTheme(Xamarin.Forms.NavigationPage.BarTextColorProperty, Color.Black, Color.White);

            MainPage = navigationPage;
        }

        protected override async void OnStart()
        {
            await IdentityService.Instance.Initialize();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

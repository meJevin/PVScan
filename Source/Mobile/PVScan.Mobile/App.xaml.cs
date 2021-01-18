using PVScan.Mobile.Services.Identity;
using PVScan.Mobile.Views;
using System;
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
            InitializeComponent();

            var navigationPage = new Xamarin.Forms.NavigationPage(new MainPage());
            navigationPage.On<iOS>().SetHideNavigationBarSeparator(false);

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

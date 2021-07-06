using Autofac;
using PVScan.Core;
using PVScan.Core.Services.Interfaces;
using PVScan.Mobile.Services;
using PVScan.Core.Services.Interfaces;
using PVScan.Mobile.Views;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Svg;
using Xamarin.Forms.Xaml;
using PVScan.Mobile.Services.Interfaces;

namespace PVScan.Mobile
{
    public partial class App : Xamarin.Forms.Application
    {
        public App()
        {
            DataAccess.Init(System.IO.Path.Combine(FileSystem.AppDataDirectory, "PVScan.db3"));

            InitializeTheme();

            InitializeComponent();

            SvgImageSource.RegisterAssembly();

            var mainPage = Resolver.Resolve<MainPage>();

            InitializePopupService(mainPage);

            var navigationPage = new Xamarin.Forms.NavigationPage(mainPage);
            navigationPage.On<iOS>().SetHideNavigationBarSeparator(false);
            navigationPage.SetOnAppTheme(Xamarin.Forms.NavigationPage.BarTextColorProperty, Color.Black, Color.White);

            MainPage = navigationPage;
        }

        private async void InitializeTheme()
        {
            using var scope = Resolver.Container.BeginLifetimeScope();
            var kvp = scope.Resolve<IPersistentKVP>();
            string themeSelected = await kvp.Get(StorageKeys.Theme, null);

            if (themeSelected == null)
            {
                // User hasn't changed the app theme yet, use system theme.
                UserAppTheme = RequestedTheme;
            }
            else
            {
                UserAppTheme = themeSelected == "Light" ? OSAppTheme.Light : OSAppTheme.Dark;
            }

            RequestedThemeChanged += (s, e) =>
            {
                // Respond to user device theme change if option is set?
            };
        }

        private void InitializePopupService(MainPage mainPage)
        {
            var popupService = (Resolver.Resolve<IPopupMessageService>() as PopupMessageService);
            popupService.Initialize(mainPage.PopupMessageBox, mainPage.PopupMessageLabel);
        }

        protected override async void OnStart()
        {
            using var scope = Resolver.Container.BeginLifetimeScope();

            var identity = scope.Resolve<IIdentityService>();
            var barcodeHub = scope.Resolve<IAPIBarcodeHub>();
            var userInfoHub = scope.Resolve<IAPIUserInfoHub>();

            await identity.Initialize();

            if (identity.AccessToken != null)
            {
                await barcodeHub.Connect();
                await userInfoHub.Connect();
            }
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

using PVScan.Mobile.ViewModels;
using PVScan.Mobile.ViewModels.Messages.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PVScan.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentView
    {
        uint _animSpeed = 250;
        double _transY = 150;

        ProfilePageViewModel VM;

        public ProfilePage()
        {
            InitializeComponent();

            VM = BindingContext as ProfilePageViewModel;

            if (VM != null)
            {
                _ = Initialize();
            }

            BindingContextChanged += async (newBindingContext, _) =>
            {
                VM = newBindingContext as ProfilePageViewModel;
                if (VM != null)
                {
                    await Initialize();
                }
            };


            MessagingCenter.Subscribe(this, nameof(SuccessfulLoginMessage),
                async (LoginPageViewModel sender, SuccessfulLoginMessage args) =>
                {
                    // User logged in
                    await Initialize();
                });

            MessagingCenter.Subscribe(this, nameof(SuccessfulLogoutMessage),
                async (LoggedInPageViewModel sender, SuccessfulLogoutMessage args) =>
                {
                    // User logged out
                    await Initialize();
                });
        }

        // Init pages and stuff
        public async Task Initialize()
        {
            var vm = BindingContext as ProfilePageViewModel;

            if (vm.IsLoggedIn)
            {
                // Logged in already, show current profile page
                SignUpPage.TranslationY = _transY;
                SignUpPage.Opacity = 0;
                SignUpPage.IsVisible = false;

                LoginPage.TranslationY = _transY;
                LoginPage.Opacity = 0;
                LoginPage.IsVisible = false;

                await LoginPage.FadeTo(0, _animSpeed, Easing.CubicOut);

                await LoggedInPage.Initialize();
            }
            else
            {
                if (LoginPage.IsVisible == false && SignUpPage.IsVisible == false)
                {
                    SignUpPage.TranslationY = _transY;
                    SignUpPage.Opacity = 0;
                    SignUpPage.IsVisible = false;

                    LoginPage.TranslationY = _transY;
                    LoginPage.Opacity = 0;
                    LoginPage.IsVisible = false;

                    LoggedInPage.Uninitialize();

                    LoginPage.IsVisible = true;

                    _ = LoginPage.TranslateTo(0, 0, _animSpeed, Easing.CubicOut);
                    await LoginPage.FadeTo(1, _animSpeed, Easing.CubicOut);
                }
            }
        }

        private async void LoginPage_SignUpClicked(object sender, EventArgs e)
        {
            SignUpPage.IsVisible = true;
            LoginPage.IsVisible = false;

            _ = SignUpPage.TranslateTo(0, 0, _animSpeed, Easing.CubicOut);
            _ = SignUpPage.FadeTo(1, _animSpeed, Easing.CubicOut);

            _ = LoginPage.TranslateTo(0, _transY, _animSpeed, Easing.CubicOut);
            await LoginPage.FadeTo(0, _animSpeed, Easing.CubicOut);
        }

        private async void SignUpPage_BackClicked(object sender, EventArgs e)
        {
            LoginPage.IsVisible = true;
            SignUpPage.IsVisible = false;

            _ = LoginPage.TranslateTo(0, 0, _animSpeed, Easing.CubicOut);
            _ = LoginPage.FadeTo(1, _animSpeed, Easing.CubicOut);

            _ = SignUpPage.TranslateTo(0, _transY, _animSpeed, Easing.CubicOut);
            await SignUpPage.FadeTo(0, _animSpeed, Easing.CubicOut);
        }

        private async void AppSettingsButtonClicked(object sender, EventArgs e)
        {
            AppSettingsPage.IsVisible = true;
            await AppSettingsPage.TranslateTo(0, 0, _animSpeed, Easing.CubicOut);
        }

        private async void AppSettingsPage_BackClicked(object sender, EventArgs e)
        {
            await AppSettingsPage.TranslateTo(AppSettingsPage.Width, 0, _animSpeed, Easing.CubicOut);
            AppSettingsPage.IsVisible = false;
        }

        private void ContentView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Width))
            {
                AppSettingsPage.TranslationX = Width;
            }
        }
    }
}
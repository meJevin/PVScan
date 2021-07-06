using PVScan.Core;
using PVScan.Desktop.WPF.ViewModels;
using PVScan.Desktop.WPF.ViewModels.Messages.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PVScan.Desktop.WPF.Views
{
    /// <summary>
    /// Interaction logic for ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : ContentControl
    {
        ProfilePageViewModel VM;

        public ProfilePage()
        {
            InitializeComponent();

            if (DataContext is ProfilePageViewModel vm)
            {
                VM = vm;
            }

            DataContextChanged += (newDataContext, _) =>
            {
                if (newDataContext is ProfilePageViewModel vm)
                {
                    VM = vm;
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

        public async Task Initialize()
        {
            if (VM.IsLoggedIn)
            {
                // Logged in already, show current profile page
                SignUpPage.IsHitTestVisible = false;
                LoginPage.IsHitTestVisible = false;
                LoggedInPage.IsHitTestVisible = true;
                _ = SignUpPage.FadeTo(0, Animations.DefaultDuration);
                _ = LoginPage.FadeTo(0, Animations.DefaultDuration);
                _ = LoggedInPage.FadeTo(1, Animations.DefaultDuration);

                await LoggedInPage.Initialize();
            }
            else
            {
                if (LoginPage.Opacity == 0 && SignUpPage.Opacity == 0)
                {
                    SignUpPage.Opacity = 0;
                    SignUpPage.IsHitTestVisible = false;

                    LoggedInPage.Opacity = 0;
                    LoggedInPage.IsHitTestVisible = false;
                    _ = LoggedInPage.FadeTo(0, Animations.DefaultDuration);

                    LoginPage.Opacity = 0;
                    LoginPage.IsHitTestVisible = true;
                    await LoginPage.FadeTo(1, Animations.DefaultDuration);
                }
            }
        }

        private async void LoginPage_SignUpClicked(object sender, EventArgs e)
        {
            LoginPage.IsHitTestVisible = false;
            _ = LoginPage.FadeTo(0, Animations.DefaultDuration);

            SignUpPage.IsHitTestVisible = true;
            await SignUpPage.FadeTo(1, Animations.DefaultDuration);
        }

        private async void SignUpPage_BackClicked(object sender, EventArgs e)
        {
            SignUpPage.IsHitTestVisible = false;
            _ = SignUpPage.FadeTo(0, Animations.DefaultDuration);

            LoginPage.IsHitTestVisible = true;
            await LoginPage.FadeTo(1, Animations.DefaultDuration);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PVScan.Mobile.Views
{
    public partial class ProfilePage : ContentView
    {
        uint _animSpeed = 250;
        double _transY = 200;

        public ProfilePage()
        {
            InitializeComponent();
        }

        // Init pages and stuff
        public async Task Initialize()
        {
            SignUpPage.TranslationY = _transY;
            SignUpPage.Opacity = 0;
            SignUpPage.IsVisible = false;
            LoginPage.TranslationY = _transY;
            LoginPage.Opacity = 0;
            SignUpPage.IsVisible = false;

            _ = LoginPage.TranslateTo(0, 0, _animSpeed, Easing.CubicOut);
            await LoginPage.FadeTo(1, _animSpeed, Easing.CubicOut);
            LoginPage.IsVisible = true;
        }

        private async void LoginPage_SignUpClicked(object sender, EventArgs e)
        {
            _ = SignUpPage.TranslateTo(0, 0, _animSpeed, Easing.CubicOut);
            _ = SignUpPage.FadeTo(1, _animSpeed, Easing.CubicOut);
            SignUpPage.IsVisible = true;

            _ = LoginPage.TranslateTo(0, _transY, _animSpeed, Easing.CubicOut);
            await LoginPage.FadeTo(0, _animSpeed, Easing.CubicOut);
            LoginPage.IsVisible = false;
        }

        private async void SignUpPage_BackClicked(object sender, EventArgs e)
        {
            _ = LoginPage.TranslateTo(0, 0, _animSpeed, Easing.CubicOut);
            _ = LoginPage.FadeTo(1, _animSpeed, Easing.CubicOut);
            LoginPage.IsVisible = true;

            _ = SignUpPage.TranslateTo(0, _transY, _animSpeed, Easing.CubicOut);
            await SignUpPage.FadeTo(0, _animSpeed, Easing.CubicOut);
            SignUpPage.IsVisible = false;
        }
    }
}
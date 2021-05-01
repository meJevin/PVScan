using PVScan.Mobile.Styles;
using PVScan.Mobile.ViewModels;
using PVScan.Mobile.ViewModels.Messages.Scanning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PVScan.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        TabViewItem _currentTabView = null;

        public MainPage()
        {
            InitializeSafeArea();
            InitializeComponent();


            MessagingCenter.Subscribe(this, nameof(BarcodeScannedMessage),
                async (ScanPageViewModel vm, BarcodeScannedMessage args) =>
                {
                    await StartBarcodeScannedAnimation();
                });
        }

        private async Task StartBarcodeScannedAnimation()
        {
            Image newImage = new Image()
            {
                BackgroundColor = Color.White,
                HeightRequest = ScannedBarcodeImageContainer.Height,
                WidthRequest = ScannedBarcodeImageContainer.Height,
                TranslationY = -16,
                Scale = 0.5,
                Opacity = 0,
            };
            newImage.Source = new FontImageSource()
            {
                FontFamily = "FontAwesome",
                Glyph = IconFont.Qrcode,
                Color = Color.Black,
            };

            ScannedBarcodeImageContainer.Children.Add(newImage);

            _ = newImage.FadeTo(1, 250, Easing.CubicOut);
            await newImage.ScaleTo(1, 250, Easing.CubicOut);
            await Task.Delay(800);
            _ = newImage.TranslateTo(0, 45, 250, Easing.CubicIn);
            await newImage.ScaleTo(0, 250, Easing.CubicIn);

            ScannedBarcodeImageContainer.Children.Remove(newImage);
        }

        private async void HistoryTabItem_TabTapped(object sender, TabTappedEventArgs e)
        {
            if (_currentTabView == sender)
            {
                return;
            }

            if (_currentTabView?.Content == ScanPage)
            {
                await ScanPage.Uninitialize();
            }

            _currentTabView = sender as TabViewItem;
        }

        private async void ScanTabItem_TabTapped(object sender, TabTappedEventArgs e)
        {
            if (_currentTabView == sender)
            {
                return;
            }

            await ScanPage.Initialize();

            _currentTabView = sender as TabViewItem;
        }

        private async void ProfileTabItem_TabTapped(object sender, TabTappedEventArgs e)
        {
            if (_currentTabView == sender)
            {
                return;
            }

            if (_currentTabView?.Content == ScanPage)
            {
                await ScanPage.Uninitialize();
            }

            _currentTabView = sender as TabViewItem;
        }

        private void InitializeSafeArea()
        {
            // This doesn't work on iOS simulators because their model is your arm64, x86_64 etc..
            if (Device.RuntimePlatform == Device.iOS)
            {
                iOSSafeArea();

                // https://gist.github.com/adamawolf/3048717
                var iphoneVersion = DeviceInfo.Model.Replace("iPhone", "").Split(',');

                if (iphoneVersion.Length != 2)
                {
                    return;
                }

                var major = int.Parse(iphoneVersion[0]);
                var minor = int.Parse(iphoneVersion[1]);

                if (major <= 9)
                {
                    iOSNoSafeArea();
                    return;
                }

                if (major == 10 &&
                    (minor == 1 || minor == 2 || minor == 4 || minor == 5))
                {
                    iOSNoSafeArea();
                    return;
                }

                if (major == 12 &&
                    minor == 8)
                {
                    iOSNoSafeArea();
                    return;
                }
            }
            else if (Device.RuntimePlatform == Device.Android)
            {
                // Todo: add android specific safe area
                iOSNoSafeArea();
            }
        }

        private void iOSSafeArea()
        {
            Application.Current.Resources["TabBarCameraButtonMargin"] = new Thickness(0, 0, 0, 32);
            Application.Current.Resources["TabBarHeight"] = 76;
            Application.Current.Resources["NegativeTabBarHeight"] = -76;
        }

        private void iOSNoSafeArea()
        {
            Application.Current.Resources["TabBarCameraButtonMargin"] = new Thickness(0, 0, 0, 10);
            Application.Current.Resources["TabBarHeight"] = 54;
            Application.Current.Resources["NegativeTabBarHeight"] = -54;
        }

        private async void ContentPage_Appearing(object sender, EventArgs e)
        {
            _currentTabView = HistoryPage.Parent as TabViewItem;

            await HistoryPage.Initialize();
            await ProfilePage.Initialize();

            this.Appearing -= ContentPage_Appearing;
        }
    }
}
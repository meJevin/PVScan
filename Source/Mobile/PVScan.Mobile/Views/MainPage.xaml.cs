using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PVScan.Mobile.Views
{
    public partial class MainPage : ContentPage
    {
        TabViewItem _currentTabView = null;

        public MainPage()
        {
            InitializeComponent();

            OnAppThemeButtonClicked(null, null);
        }

        void OnAppThemeButtonClicked(object sender, System.EventArgs e)
        {
            Application.Current.UserAppTheme = (Application.Current.UserAppTheme == OSAppTheme.Dark)
                ? OSAppTheme.Light
                : OSAppTheme.Dark;
        }

        private async void HistoryTabItem_TabTapped(object sender, TabTappedEventArgs e)
        {
            if (_currentTabView == sender)
            {
                return;
            }

            _currentTabView = sender as TabViewItem;

            await HistoryPage.Initialize();
        }

        private async void ScanTabItem_TabTapped(object sender, TabTappedEventArgs e)
        { 
            var item = sender as TabViewItem;
            // Tap animation
            var tabViewOriginalScale = item.Scale;
            await item.ScaleTo(tabViewOriginalScale - 0.1, 50, Easing.Linear);
            await Task.Delay(100);
            await item.ScaleTo(tabViewOriginalScale, 50, Easing.Linear);

            await Navigation.PushAsync(new ScanPage(), true);
        }

        private async void ProfileTabItem_TabTapped(object sender, TabTappedEventArgs e)
        {
            if (_currentTabView == sender)
            {
                return;
            }

            _currentTabView = sender as TabViewItem;

            await ProfilePage.Initialize();
        }
    }
}
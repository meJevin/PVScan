using PVScan.Mobile.ViewModels;
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
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        TabViewItem _currentTabView = null;

        public MainPage()
        {
            InitializeComponent();

            MainTabView.SelectedIndex = 1;
            _currentTabView = ScanPage.Parent as TabViewItem;
            ScanPage.Initialize();
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

            await HistoryPage.Initialize();
        }

        private async void ScanTabItem_TabTapped(object sender, TabTappedEventArgs e)
        {
            if (_currentTabView == sender)
            {
                return;
            }

            _currentTabView = sender as TabViewItem;

            // Tap animation
            var tabViewOriginalScale = _currentTabView.Scale;
            await _currentTabView.ScaleTo(tabViewOriginalScale - 0.1, 50, Easing.Linear);
            await Task.Delay(100);
            await _currentTabView.ScaleTo(tabViewOriginalScale, 50, Easing.Linear);

            await ScanPage.Initialize();
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

            await ProfilePage.Initialize();
        }

    }
}
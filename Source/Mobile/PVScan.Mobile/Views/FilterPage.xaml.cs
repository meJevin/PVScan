using PVScan.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PVScan.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FilterPage : ContentView
    {
        public FilterPage()
        {
            InitializeComponent();
        }

        private void ResetDateFilter_Tapped(object sender, EventArgs e)
        {
            // https://github.com/xamarin/XamarinCommunityToolkit/issues/595
            MethodInfo dynMethod = DateFilterTypeTabView.GetType().GetMethod("UpdateSelectedIndex", BindingFlags.NonPublic | BindingFlags.Instance);
            dynMethod?.Invoke(DateFilterTypeTabView, new object[] { 0, false });
        }
    }
}
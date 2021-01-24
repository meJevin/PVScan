using PVScan.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PVScan.Mobile.Views
{
    public partial class HistoryPage : ContentView
    {
        public HistoryPage()
        {
            InitializeComponent();
        }

        public async Task Initialize()
        {
            await (BindingContext as HistoryPageViewModel).Initialize();
        }
    }
}
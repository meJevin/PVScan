using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PVScan.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpPage : ContentView
    {
        public event EventHandler BackClicked;

        public SignUpPage()
        {
            InitializeComponent();
        }

        private async void BackClicked_Handler(object sender, EventArgs e)
        {
            BackClicked.Invoke(sender, e);
        }
    }
}
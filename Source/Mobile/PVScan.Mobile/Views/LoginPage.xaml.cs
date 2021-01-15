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
    public partial class LoginPage : ContentView
    {
        public event EventHandler SignUpClicked;

        public LoginPage()
        {
            InitializeComponent();
        }

        private void SignUpClicked_Handler(object sender, EventArgs e)
        {
            SignUpClicked.Invoke(sender, e);
        }
    }
}
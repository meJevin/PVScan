using PVScan.Mobile.ViewModels;
using PVScan.Mobile.ViewModels.Messages;
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

            MessagingCenter.Subscribe(this, nameof(FailedLoginMessage),
                async (LoginPageViewModel sender, FailedLoginMessage args) =>
                {
                    LoginMessageLabel.Text = args.Message;

                    // Update UI for successful Sign Up
                    await LoginMessageLabel.FadeTo(1);

                    await Task.Delay(1500);

                    await LoginMessageLabel.FadeTo(0);
                });
        }

        private void SignUpClicked_Handler(object sender, EventArgs e)
        {
            SignUpClicked.Invoke(sender, e);
        }
    }
}
using PVScan.Mobile.ViewModels;
using PVScan.Mobile.ViewModels.Messages.Auth;
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

            MessagingCenter.Subscribe(this, nameof(SuccessfulSignUpMessage),
                async (SignUpPageViewModel sender, SuccessfulSignUpMessage args) =>
                {
                    SignUpMessageLabel.Text = args.Message;

                    // Update UI for successful Sign Up
                    await SignUpMessageLabel.FadeTo(1);

                    await Task.Delay(1500);

                    await SignUpMessageLabel.FadeTo(0);
                });

            MessagingCenter.Subscribe(this, nameof(FailedSignUpMessage),
                async (SignUpPageViewModel sender, FailedSignUpMessage args) =>
                {
                    SignUpMessageLabel.Text = args.Message;

                    // Update UI for failed Sign Up
                    await SignUpMessageLabel.FadeTo(1);

                    await Task.Delay(1500);

                    await SignUpMessageLabel.FadeTo(0);
                });
        }

        private async void BackClicked_Handler(object sender, EventArgs e)
        {
            BackClicked.Invoke(sender, e);
        }
    }
}
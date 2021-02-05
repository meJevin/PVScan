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

            var vm = (BindingContext as SignUpPageViewModel);

            vm.SuccessfulSignUp += Vm_SuccessfulSignUp;
            vm.FailedSignUp += Vm_FailedSignUp;
        }

        private async void Vm_FailedSignUp(object sender, SignUpEventArgs e)
        {
            SignUpMessageLabel.Text = e.Message;

            // Update UI for successful Sign Up

            await SignUpMessageLabel.FadeTo(1);
            await Task.Delay(1500);
            await SignUpMessageLabel.FadeTo(0);
        }

        private async void Vm_SuccessfulSignUp(object sender, SignUpEventArgs e)
        {
            SignUpMessageLabel.Text = e.Message;

            // Update UI for failed Sign Up

            await SignUpMessageLabel.FadeTo(1);
            await Task.Delay(1500);
            await SignUpMessageLabel.FadeTo(0);
        }

        private async void BackClicked_Handler(object sender, EventArgs e)
        {
            BackClicked.Invoke(sender, e);
        }
    }
}
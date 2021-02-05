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
    public partial class LoginPage : ContentView
    {
        public event EventHandler SignUpClicked;

        public LoginPage()
        {
            InitializeComponent();

            var vm = (BindingContext as LoginPageViewModel);

            vm.SuccessfulLogin += Vm_SuccessfulLogin;
            vm.FailedLogin += Vm_FailedLogin;
        }

        private async void Vm_FailedLogin(object sender, LoginEventArgs e)
        {
            LoginMessageLabel.Text = e.Message;

            _ = LoginMessageLabel.FadeTo(1);

            await Task.Delay(1500);

            _ = LoginMessageLabel.FadeTo(0);
        }

        private void Vm_SuccessfulLogin(object sender, LoginEventArgs e)
        {
        }

        private void SignUpClicked_Handler(object sender, EventArgs e)
        {
            SignUpClicked.Invoke(sender, e);
        }
    }
}
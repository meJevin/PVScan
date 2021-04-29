using PVScan.Mobile.ViewModels;
using PVScan.Mobile.ViewModels.Messages.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        LoginPageViewModel VM;

        public LoginPage()
        {
            InitializeComponent();

            VM = (BindingContext as LoginPageViewModel);

            VM.SuccessfulLogin += Vm_SuccessfulLogin;
            VM.FailedLogin += Vm_FailedLogin;

            VM.PropertyChanged += Vm_PropertyChanged;
        }

        private async void Vm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LoginPageViewModel.IsLoggingIn))
            {
                if (VM.IsLoggingIn)
                {
                    // Show spinner
                    _ = LoadingSpinner.FadeTo(1, 250, Easing.CubicOut);
                    await LoadingSpinner.ScaleTo(1, 250, Easing.CubicOut);
                }
                else
                {
                    // Hide spinner
                    _ = LoadingSpinner.FadeTo(0, 250, Easing.CubicOut);
                    await LoadingSpinner.ScaleTo(0.75, 250, Easing.CubicOut);
                }
            }
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
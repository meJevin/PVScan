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
    public partial class SignUpPage : ContentView
    {
        public event EventHandler BackClicked;

        SignUpPageViewModel VM;

        public SignUpPage()
        {
            InitializeComponent();

            VM = (BindingContext as SignUpPageViewModel);

            VM.SuccessfulSignUp += Vm_SuccessfulSignUp;
            VM.FailedSignUp += Vm_FailedSignUp;

            VM.PropertyChanged += VM_PropertyChanged;
        }

        private async void VM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SignUpPageViewModel.IsSigningUp))
            {
                if (VM.IsSigningUp)
                {
                    // Show spinner
                    LoginEntry.Opacity = 0.5;
                    EmailEntry.Opacity = 0.5;
                    PasswordEntry.Opacity = 0.5;
                    SignUpButton.Opacity = 0.5;
                    LoginEntry.InputTransparent = true;
                    EmailEntry.InputTransparent = true;
                    PasswordEntry.InputTransparent = true;
                    SignUpButton.InputTransparent = true;
                    _ = LoadingSpinner.FadeTo(1, 250, Easing.CubicOut);
                    await LoadingSpinner.ScaleTo(1, 250, Easing.CubicOut);
                }
                else
                {
                    // Hide spinner
                    LoginEntry.Opacity = 1;
                    EmailEntry.Opacity = 1;
                    PasswordEntry.Opacity =1;
                    SignUpButton.Opacity = 1;
                    LoginEntry.InputTransparent = false;
                    EmailEntry.InputTransparent = false;
                    PasswordEntry.InputTransparent = false;
                    SignUpButton.InputTransparent = false;
                    _ = LoadingSpinner.FadeTo(0, 250, Easing.CubicOut);
                    await LoadingSpinner.ScaleTo(0.75, 250, Easing.CubicOut);
                }
            }
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
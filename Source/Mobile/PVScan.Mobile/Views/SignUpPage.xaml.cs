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
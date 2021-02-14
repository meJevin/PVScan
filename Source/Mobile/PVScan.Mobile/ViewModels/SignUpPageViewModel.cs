using MvvmHelpers;
using PVScan.Mobile.Services.Interfaces;
using PVScan.Mobile.ViewModels.Messages.Auth;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace PVScan.Mobile.ViewModels
{
    public class SignUpEventArgs
    {
        public string Message { get; set; }
    }

    public class SignUpPageViewModel : BaseViewModel
    {
        readonly IIdentityService IdentityService;

        public SignUpPageViewModel(IIdentityService identityService)
        {
            IdentityService = identityService;

            SignUpCommand = new Command(async () =>
            {
                var result = await IdentityService.SignUpAsync(Login, Password, Email);

                if (result)
                {
                    SuccessfulSignUp?.Invoke(this, new SignUpEventArgs()
                    {
                        Message = "You've successfuly signed up",
                    });
                }
                else
                {
                    FailedSignUp?.Invoke(this, new SignUpEventArgs()
                    {
                        Message = "Failed to sign up!",
                    });
                }
            });
        }

        public ICommand SignUpCommand { get; }

        public event EventHandler<SignUpEventArgs> SuccessfulSignUp;
        public event EventHandler<SignUpEventArgs> FailedSignUp;

        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

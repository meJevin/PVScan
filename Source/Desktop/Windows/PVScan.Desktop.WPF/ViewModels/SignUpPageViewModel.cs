using PVScan.Core.Services.Interfaces;
using PVScan.Desktop.WPF.Services.Interfaces;
using PVScan.Desktop.WPF.Views.Popups;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace PVScan.Desktop.WPF.ViewModels
{
    public class SignUpEventArgs
    {
        public string Message { get; set; }
    }

    public class SignUpPageViewModel : BaseViewModel
    {
        readonly IIdentityService IdentityService;
        readonly IPopup<TextMessagePopupArgs, TextMessagePopupResult> TextMessagePopup;

        public SignUpPageViewModel(
            IIdentityService identityService,
            IPopup<TextMessagePopupArgs, TextMessagePopupResult> popupMessageService)
        {
            IdentityService = identityService;
            TextMessagePopup = popupMessageService;

            SignUpCommand = new Command(async () =>
            {
                if (string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Email))
                {
                    // Todo: dead code
                    FailedSignUp?.Invoke(this, new SignUpEventArgs()
                    {
                        Message = "Please fill in all fields!",
                    });

                    await TextMessagePopup.ShowPopup(new TextMessagePopupArgs()
                    {
                        Message = "Please fill in all fields!",
                    });

                    return;
                }

                IsSigningUp = true;

                var result = await IdentityService.SignUpAsync(Login, Password, Email);

                IsSigningUp = false;

                if (result)
                {
                    // Todo: dead code
                    SuccessfulSignUp?.Invoke(this, new SignUpEventArgs()
                    {
                        Message = "You've successfuly signed up",
                    });

                    await TextMessagePopup.ShowPopup(new TextMessagePopupArgs()
                    {
                        Message = "You've successfuly signed up!",
                    });
                }
                else
                {
                    // Todo: dead code
                    FailedSignUp?.Invoke(this, new SignUpEventArgs()
                    {
                        Message = "Failed to sign up!",
                    });

                    await TextMessagePopup.ShowPopup(new TextMessagePopupArgs()
                    {
                        Message = "Failed to sign up!",
                    });
                }

                Login = "";
                Email = "";
                Password = "";
            });
        }

        public ICommand SignUpCommand { get; }

        public event EventHandler<SignUpEventArgs> SuccessfulSignUp;
        public event EventHandler<SignUpEventArgs> FailedSignUp;

        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public bool IsSigningUp { get; set; }
    }
}

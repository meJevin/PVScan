using MvvmHelpers;
using PVScan.Mobile.Services.Interfaces;
using PVScan.Mobile.ViewModels.Messages.Auth;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace PVScan.Mobile.ViewModels
{
    public class LoginEventArgs
    {
        public string Message { get; set; }
    }

    public class LoginPageViewModel : BaseViewModel
    {
        readonly IIdentityService IdentityService;

        public LoginPageViewModel(IIdentityService identityService)
        {
            IdentityService = identityService;

            LoginCommand = new Command(async () =>
            {
                var result = await IdentityService.LoginAsync(Login, Password);

                if (result)
                {
                    SuccessfulLogin?.Invoke(this, new LoginEventArgs()
                    {
                        Message = "You've succesfuly logged in!",
                    });

                    // Profile view responds to this changing the UI
                    MessagingCenter.Send(this, nameof(SuccessfulLoginMessage), new SuccessfulLoginMessage() { });
                }
                else
                {
                    FailedLogin?.Invoke(this, new LoginEventArgs()
                    {
                        Message = "Failed to login!",
                    });
                }
            });
        }

        public ICommand LoginCommand { get; }

        public event EventHandler<LoginEventArgs> SuccessfulLogin;
        public event EventHandler<LoginEventArgs> FailedLogin;

        public string Login { get; set; }
        public string Password { get; set; }
    }
}

using MvvmHelpers;
using PVScan.Mobile.Services.Identity;
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
        public IIdentityService identityService;

        public LoginPageViewModel()
        {
            identityService = IdentityService.Instance;

            LoginCommand = new Command(async () =>
            {
                var result = await identityService.LoginAsync(Login, Password);

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

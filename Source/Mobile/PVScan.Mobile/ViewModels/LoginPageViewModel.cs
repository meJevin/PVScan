using MvvmHelpers;
using PVScan.Core.Services.Interfaces;
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
        readonly IPopupMessageService PopupMessageService;
        readonly IAPIBarcodeHub BarcodeHub;
        readonly IAPIUserInfoHub UserInfoHub;

        public LoginPageViewModel(
            IIdentityService identityService,
            IPopupMessageService popupMessageService,
            IAPIBarcodeHub barcodeHub, IAPIUserInfoHub userInfoHub)
        {
            IdentityService = identityService;
            PopupMessageService = popupMessageService;
            BarcodeHub = barcodeHub;
            UserInfoHub = userInfoHub;

            LoginCommand = new Command(async () =>
            {
                if (string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(Password))
                {
                    _ = PopupMessageService?.ShowMessage("Please fill in all fields!");

                    FailedLogin?.Invoke(this, new LoginEventArgs() { });

                    return;
                }

                IsLoggingIn = true;

                var result = await IdentityService.LoginAsync(Login, Password);

                IsLoggingIn = false;

                Login = "";
                Password = "";

                if (result)
                {
                    // Profile view responds to this changing the UI
                    MessagingCenter.Send(this, nameof(SuccessfulLoginMessage), new SuccessfulLoginMessage() { });

                    await BarcodeHub.Connect();
                    await UserInfoHub.Connect();

                    // Todo: dead code :)
                    SuccessfulLogin?.Invoke(this, new LoginEventArgs() { });
                }
                else
                {
                    _ = PopupMessageService?.ShowMessage("Failed to login!");

                    // Todo: dead code :)
                    FailedLogin?.Invoke(this, new LoginEventArgs() { });
                }
            });
        }

        public ICommand LoginCommand { get; }

        public event EventHandler<LoginEventArgs> SuccessfulLogin;
        public event EventHandler<LoginEventArgs> FailedLogin;

        public string Login { get; set; }
        public string Password { get; set; }

        public bool IsLoggingIn { get; set; }
    }
}

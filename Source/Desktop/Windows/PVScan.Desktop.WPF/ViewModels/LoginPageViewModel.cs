using PVScan.Core.Services.Interfaces;
using PVScan.Desktop.WPF.Services.Interfaces;
using PVScan.Desktop.WPF.ViewModels.Messages.Auth;
using PVScan.Desktop.WPF.Views.Popups;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace PVScan.Desktop.WPF.ViewModels
{
    public class LoginEventArgs
    {
        public string Message { get; set; }
    }

    public class LoginPageViewModel : BaseViewModel
    {
        readonly IIdentityService IdentityService;
        readonly IPopup<TextMessagePopupArgs, TextMessagePopupResult> TextMessagePopup;
        readonly IAPIBarcodeHub BarcodeHub;
        readonly IAPIUserInfoHub UserInfoHub;
        readonly IBarcodeSynchronizer Synchronizer;

        public LoginPageViewModel(
            IIdentityService identityService,
            IPopup<TextMessagePopupArgs, TextMessagePopupResult> popupMessageService,
            IAPIBarcodeHub barcodeHub, 
            IAPIUserInfoHub userInfoHub, 
            IBarcodeSynchronizer synchronizer)
        {
            IdentityService = identityService;
            TextMessagePopup = popupMessageService;
            BarcodeHub = barcodeHub;
            UserInfoHub = userInfoHub;
            Synchronizer = synchronizer;

            LoginCommand = new Command(async () =>
            {
                if (string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(Password))
                {
                    FailedLogin?.Invoke(this, new LoginEventArgs() { });

                    await TextMessagePopup?.ShowPopup(new TextMessagePopupArgs()
                    {
                        Message = "Please fill in all fields!",
                    });

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

                    await Synchronizer.Synchronize();

                    // Todo: dead code :)
                    SuccessfulLogin?.Invoke(this, new LoginEventArgs() { });
                }
                else
                {
                    // Todo: dead code :)
                    FailedLogin?.Invoke(this, new LoginEventArgs() { });

                    await TextMessagePopup?.ShowPopup(new TextMessagePopupArgs()
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

        public bool IsLoggingIn { get; set; }
    }
}

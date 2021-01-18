using MvvmHelpers;
using PVScan.Mobile.Services.Identity;
using PVScan.Mobile.ViewModels.Messages;
using System.Windows.Input;
using Xamarin.Forms;

namespace PVScan.Mobile.ViewModels
{
    public class LoginPageViewModel : BaseViewModel
    {
        readonly IIdentityService identityService;

        public LoginPageViewModel()
        {
            identityService = IdentityService.Instance;

            LoginCommand = new Command(async () =>
            {
                var result = await identityService.LoginAsync(Login, Password);

                if (result)
                {
                    MessagingCenter.Send(this, nameof(SuccessfulLoginMessage), new SuccessfulLoginMessage()
                    {
                        Message = "",
                    });
                }
                else
                {
                    MessagingCenter.Send(this, nameof(FailedLoginMessage), new FailedLoginMessage()
                    {
                        // Todo: add error messsage from server
                        Message = "Could not log in!",
                    });
                }
            });
        }

        public ICommand LoginCommand { get; }

        public string Login { get; set; }
        public string Password { get; set; }
    }
}

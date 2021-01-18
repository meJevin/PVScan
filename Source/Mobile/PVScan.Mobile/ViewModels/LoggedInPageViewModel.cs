using MvvmHelpers;
using PVScan.Mobile.Models;
using PVScan.Mobile.Services.Identity;
using PVScan.Mobile.ViewModels.Messages;
using System.Windows.Input;
using Xamarin.Forms;

namespace PVScan.Mobile.ViewModels
{
    public class LoggedInPageViewModel : BaseViewModel
    {
        readonly IIdentityService identityService;

        public LoggedInPageViewModel()
        {
            identityService = IdentityService.Instance;

            LogoutCommand = new Command(async () =>
            {
                var result = await identityService.LogoutAsync();

                if (result)
                {
                    MessagingCenter.Send(this, nameof(SuccessfulLogoutMessage), new SuccessfulLogoutMessage()
                    {
                    });
                }
                else
                {
                    // Todo: handle logout failure
                    MessagingCenter.Send(this, nameof(FailedLogoutMessage), new FailedLogoutMessage()
                    {
                    });
                }
            });
        }

        public ICommand LogoutCommand { get; }

        public UserInfo UserInfo { get; set; }
    }
}

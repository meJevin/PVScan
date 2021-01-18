using MvvmHelpers;
using PVScan.Mobile.Services.Identity;
using System.Windows.Input;
using Xamarin.Forms;

namespace PVScan.Mobile.ViewModels
{
    public class LoginPageViewModel : BaseViewModel
    {
        readonly IIdentityService identityService;

        public LoginPageViewModel()
        {
            identityService = new IdentityService();

            LoginCommand = new Command(async () =>
            {
            });
        }

        public ICommand LoginCommand { get; }

        public string Login { get; set; }
        public string Password { get; set; }
    }
}

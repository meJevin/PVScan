using MvvmHelpers;
using PVScan.Mobile.Services.Identity;
using PVScan.Mobile.ViewModels.Messages;
using System.Windows.Input;
using Xamarin.Forms;

namespace PVScan.Mobile.ViewModels
{
    public class SignUpPageViewModel : BaseViewModel
    {
        readonly IIdentityService identityService;

        public SignUpPageViewModel()
        {
            identityService = IdentityService.Instance;

            SignUpCommand = new Command(async () =>
            {
                var result = await identityService.SignUpAsync(Login, Password, Email);

                if (result)
                {
                    MessagingCenter.Send(this, nameof(SuccessfulSignUpMessage), new SuccessfulSignUpMessage()
                    {
                        Message = "You've successfuly signed up!",
                    });
                }
                else
                {
                    MessagingCenter.Send(this, nameof(FailedSignUpMessage), new FailedSignUpMessage()
                    {
                        // Todo: add error messsage from server
                        Message = "Could not sign up!",
                    });
                }
            });
        }

        public ICommand SignUpCommand { get;  }

        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

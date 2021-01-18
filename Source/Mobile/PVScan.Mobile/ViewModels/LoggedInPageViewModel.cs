using MvvmHelpers;
using Newtonsoft.Json;
using PVScan.Mobile.Models;
using PVScan.Mobile.Services.Identity;
using PVScan.Mobile.ViewModels.Messages;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
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

            SaveProfileCommand = new Command(async () =>
            {
                // Update profile
                var client = HttpClientUtils.APIHttpClientWithToken(identityService.AccessToken);

                var content = new StringContent(JsonConvert.SerializeObject(UserInfo), Encoding.UTF8, "application/json");

                var response = await client.PostAsync("api/v1/users/change", content);

                if (response.IsSuccessStatusCode)
                {
                    // Good
                    identityService.CurrentUserInfo.IGLink = UserInfo.IGLink;
                    identityService.CurrentUserInfo.VKLink = UserInfo.VKLink;
                }

                Initialize();
            });
        }

        public void Initialize()
        {
            UserInfo = new UserInfo()
            {
                Email = identityService.CurrentUserInfo.Email,
                Username = identityService.CurrentUserInfo.Username,
                BarcodeFormatsScanned = identityService.CurrentUserInfo.BarcodeFormatsScanned,
                BarcodesScanned = identityService.CurrentUserInfo.BarcodesScanned,
                Experience = identityService.CurrentUserInfo.Experience,
                Level = identityService.CurrentUserInfo.Level,
                IGLink = identityService.CurrentUserInfo.IGLink,
                VKLink = identityService.CurrentUserInfo.VKLink,
            };

            OnPropertyChanged("UserInfo");
        }

        public ICommand RefreshCommand { get; }

        public ICommand SaveProfileCommand { get; }
        
        public ICommand LogoutCommand { get; }

        public UserInfo UserInfo { get; set; }
    }
}

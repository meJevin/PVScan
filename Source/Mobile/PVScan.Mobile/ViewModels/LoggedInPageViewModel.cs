using MvvmHelpers;
using Newtonsoft.Json;
using PVScan.Mobile.Models;
using PVScan.Mobile.Services.Identity;
using PVScan.Mobile.ViewModels.Messages;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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

                await Initialize();
            });

            RefreshCommand = new Command(async () =>
            {
                IsRefreshing = true;
                OnPropertyChanged(nameof(IsRefreshing));

                await Initialize();

                IsRefreshing = false;
                OnPropertyChanged(nameof(IsRefreshing));
            });
        }

        public async Task Initialize()
        {
            HttpClient httpClient = HttpClientUtils.APIHttpClientWithToken(identityService.AccessToken);
            var result = await httpClient.GetAsync("api/v1/users/current");

            if (!result.IsSuccessStatusCode)
            {
                // Send message to UI that there's an error
                return;
            }

            var strContent = await result.Content.ReadAsStringAsync();

            UserInfo = JsonConvert.DeserializeObject<UserInfo>(strContent);

            OnPropertyChanged(nameof(UserInfo));
        }

        public bool IsRefreshing { get; set; } = false;

        public ICommand RefreshCommand { get; }

        public ICommand SaveProfileCommand { get; }
        
        public ICommand LogoutCommand { get; }

        public UserInfo UserInfo { get; set; }
    }
}

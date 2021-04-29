using MvvmHelpers;
using Newtonsoft.Json;
using PVScan.Mobile.Models;
using PVScan.Mobile.Services;
using PVScan.Mobile.Services.Interfaces;
using PVScan.Mobile.ViewModels.Messages.Auth;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PVScan.Mobile.ViewModels
{
    public class LogoutEventArgs
    {
        public string Message { get; set; }
    }

    public class LoggedInPageViewModel : BaseViewModel
    {
        readonly IIdentityService IdentityService;
        readonly IHttpClientFactory HttpFactory;

        public LoggedInPageViewModel(IIdentityService identityService, IHttpClientFactory httpFactory)
        {
            IdentityService = identityService;
            HttpFactory = httpFactory;

            LogoutCommand = new Command(async () =>
            {
                IsLogginOut = true;
                var result = await IdentityService.LogoutAsync();
                IsLogginOut = false;

                if (result)
                {
                    SuccessfulLogout.Invoke(this, new LogoutEventArgs() { });

                    MessagingCenter.Send(this, nameof(SuccessfulLogoutMessage), new SuccessfulLogoutMessage()
                    {
                    });
                }
                else
                {
                    FailedLogout.Invoke(this, new LogoutEventArgs() { });
                }
            });

            SaveProfileCommand = new Command(async () =>
            {
                // Update profile
                var client = HttpFactory.ForAPI(IdentityService.AccessToken);

                var content = new StringContent(JsonConvert.SerializeObject(UserInfo), Encoding.UTF8, "application/json");

                IsUpdatingUserInfo = true;
                var response = await client.PostAsync("api/v1/users/change", content);
                IsUpdatingUserInfo = false;

                await Initialize();
            });

            RefreshCommand = new Command(async () =>
            {
                IsRefreshing = true;

                await Initialize();

                IsRefreshing = false;
            });
        }

        public async Task Initialize()
        {
            HttpClient httpClient = HttpFactory.ForAPI(IdentityService.AccessToken);

            IsUpdatingUserInfo = true;
            var result = await httpClient.GetAsync("api/v1/users/current");
            IsUpdatingUserInfo = false;

            if (!result.IsSuccessStatusCode)
            {
                // Send message to UI that there's an error
                return;
            }

            var strContent = await result.Content.ReadAsStringAsync();

            UserInfo = JsonConvert.DeserializeObject<UserInfo>(strContent);
        }

        public bool IsRefreshing { get; set; }

        public ICommand RefreshCommand { get; }

        public ICommand SaveProfileCommand { get; }
        
        public ICommand LogoutCommand { get; }
        public event EventHandler<LogoutEventArgs> SuccessfulLogout;
        public event EventHandler<LogoutEventArgs> FailedLogout;

        public UserInfo UserInfo { get; set; }

        public bool IsUpdatingUserInfo { get; set; }
        public bool IsLogginOut { get; set; }
    }
}

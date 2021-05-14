using MvvmHelpers;
using Newtonsoft.Json;
using PVScan.Mobile.Models;
using PVScan.Mobile.Models.API;
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
        readonly IPVScanAPI API;
        readonly IAPIBarcodeHub BarcodeHub;

        public LoggedInPageViewModel(IIdentityService identityService, IPVScanAPI api, IAPIBarcodeHub barcodeHub)
        {
            IdentityService = identityService;
            API = api;
            BarcodeHub = barcodeHub;

            LogoutCommand = new Command(async () =>
            {
                IsLogginOut = true;

                IsError = false;

                var result = await IdentityService.LogoutAsync();

                if (result == false)
                {
                    // Could not logout :(
                    IsLogginOut = false;
                    IsError = true;
                    return;
                }

                UserInfo = null;
                IsLogginOut = false;

                if (result)
                {
                    SuccessfulLogout.Invoke(this, new LogoutEventArgs() { });

                    MessagingCenter.Send(this, nameof(SuccessfulLogoutMessage), new SuccessfulLogoutMessage()
                    {
                    });

                    await BarcodeHub.Disconnect();
                }
                else
                {
                    FailedLogout.Invoke(this, new LogoutEventArgs() { });
                }
            });

            SaveProfileCommand = new Command(async () =>
            {
                string prevIGLink = UserInfo.IGLink;
                string prevVKLink = UserInfo.VKLink;

                IsUpdatingUserInfo = true;

                IsError = false;

                var result = await API.ChangeUserInfo(new ChangeUserInfoRequest
                {
                    IGLink = UserInfo.IGLink,
                    VKLink = UserInfo.VKLink,
                });

                IsUpdatingUserInfo = false;

                if (result == null)
                {
                    // Whoops, something went wrong ...
                    UserInfo.VKLink = prevIGLink;
                    UserInfo.IGLink = prevIGLink;
                    IsError = true;
                    return;
                }
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
            IsUpdatingUserInfo = true;
            IsInitializing = true;

            IsError = false;

            var user = await API.GetUserInfo(new GetUserInfoRequest() { });

            if (user == null)
            {
                // Could not get user info :(
                IsInitializing = false;
                IsUpdatingUserInfo = false;
                IsError = true;

                return;
            }

            UserInfo = new UserInfo()
            {
                BarcodeFormatsScanned = user.BarcodeFormatsScanned,
                BarcodesScanned = user.BarcodesScanned,
                Email = user.Email,
                Experience = user.Experience,
                IGLink = user.IGLink,
                VKLink = user.VKLink,
                Level = user.Level,
                Username = user.Username,
            };

            IsInitializing = false;
            IsUpdatingUserInfo = false;
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
        public bool IsInitializing { get; set; }

        public bool IsError { get; set; }
    }
}

using PVScan.Core.Models;
using PVScan.Core.Models.API;
using PVScan.Core.Services.Interfaces;
using PVScan.Desktop.WPF.ViewModels.Messages.Auth;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PVScan.Desktop.WPF.ViewModels
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
        readonly IAPIUserInfoHub UserInfoHub;

        public LoggedInPageViewModel(IIdentityService identityService, IPVScanAPI api,
            IAPIBarcodeHub barcodeHub, IAPIUserInfoHub userInfoHub)
        {
            IdentityService = identityService;
            API = api;
            BarcodeHub = barcodeHub;
            UserInfoHub = userInfoHub;

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
                    SuccessfulLogout?.Invoke(this, new LogoutEventArgs() { });

                    MessagingCenter.Send(this, nameof(SuccessfulLogoutMessage), new SuccessfulLogoutMessage()
                    {
                    });

                    await BarcodeHub.Disconnect();
                    await UserInfoHub.Disconnect();
                }
                else
                {
                    FailedLogout?.Invoke(this, new LogoutEventArgs() { });
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

                if (result != null)
                {
                    await UserInfoHub.Changed(new GetUserInfoResponse()
                    {
                        BarcodeFormatsScanned = UserInfo.BarcodeFormatsScanned,
                        BarcodesScanned = UserInfo.BarcodesScanned,
                        Email = UserInfo.Email,
                        Experience = UserInfo.Experience,
                        IGLink = UserInfo.IGLink,
                        VKLink = UserInfo.VKLink,
                        Level = UserInfo.Level,
                        Username = UserInfo.Username,
                    });
                }

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

            UserInfoHub.OnChanged += UserInfoHub_OnChanged;
        }

        private void UserInfoHub_OnChanged(object sender, GetUserInfoResponse newUserInfo)
        {
            UserInfo = new UserInfo()
            {
                BarcodeFormatsScanned = newUserInfo.BarcodeFormatsScanned,
                BarcodesScanned = newUserInfo.BarcodesScanned,
                Experience = newUserInfo.Experience,
                IGLink = newUserInfo.IGLink,
                VKLink = newUserInfo.VKLink,
                Level = newUserInfo.Level,
                Email = UserInfo.Email,
                Username = UserInfo.Username,
            };
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

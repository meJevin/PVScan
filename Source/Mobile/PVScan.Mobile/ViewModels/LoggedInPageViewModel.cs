﻿using MvvmHelpers;
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

        public LoggedInPageViewModel(IIdentityService identityService, IPVScanAPI api)
        {
            IdentityService = identityService;
            API = api;

            LogoutCommand = new Command(async () =>
            {
                IsLogginOut = true;

                var result = await IdentityService.LogoutAsync();
                UserInfo = null;

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
                string prevIGLink = UserInfo.IGLink;
                string prevVKLink = UserInfo.VKLink;

                IsUpdatingUserInfo = true;
                
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

            var user = await API.GetUserInfo(new GetUserInfoRequest() { });

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
    }
}

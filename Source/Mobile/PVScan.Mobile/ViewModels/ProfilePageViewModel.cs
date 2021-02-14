using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using MvvmHelpers;
using PVScan.Mobile.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PVScan.Mobile.ViewModels
{
    public class ProfilePageViewModel : BaseViewModel
    {
        readonly IIdentityService IdentityService;

        public ProfilePageViewModel(IIdentityService identityService)
        {
            IdentityService = identityService;
        }

        public bool IsLoggedIn
        {
            get
            {
                return IdentityService.AccessToken != null;
            }
        }
    }
}

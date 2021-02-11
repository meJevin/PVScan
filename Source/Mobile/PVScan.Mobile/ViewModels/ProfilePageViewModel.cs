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
        public IIdentityService IdentityService { get; set; }

        public ProfilePageViewModel()
        {
            IdentityService = Resolver.Resolve<IIdentityService>();
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

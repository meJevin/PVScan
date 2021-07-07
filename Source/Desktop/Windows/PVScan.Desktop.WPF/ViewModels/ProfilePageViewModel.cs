using PVScan.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PVScan.Desktop.WPF.ViewModels
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

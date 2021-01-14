using IdentityModel.OidcClient;
using PVScan.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Mobile.Services.Identity
{
    public interface IIdentityService
    {
        Task<LoginResult> LoginAsync();

        Task<LogoutResult> LogoutAsync();

        bool IsLoggedIn { get; }

        OidcClient Client { get; }
    }
}

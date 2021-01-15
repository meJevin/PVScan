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
        Task LoginAsync(string username, string password);

        Task LogoutAsync();

        Task SignUpAsync(string userName, string password);

        bool IsLoggedIn { get; }
    }
}

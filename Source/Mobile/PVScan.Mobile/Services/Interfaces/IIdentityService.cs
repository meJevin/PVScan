using IdentityModel.OidcClient;
using PVScan.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Mobile.Services.Interfaces
{
    // Todo: remake this to support errors reported from backend about different failures
    public interface IIdentityService
    {
        Task Initialize();

        Task<bool> LoginAsync(string username, string password);

        Task<bool> LogoutAsync();

        Task<bool> SignUpAsync(string username, string password, string email);

        string AccessToken { get; }
    }
}

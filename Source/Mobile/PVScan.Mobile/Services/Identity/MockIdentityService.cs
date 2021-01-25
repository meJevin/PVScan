using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Mobile.Services.Identity
{
    public class MockIdentityService : IIdentityService
    {
        public string AccessToken => null;

        public async Task Initialize()
        {
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            return true;
        }

        public async Task<bool> LogoutAsync()
        {
            return true;
        }

        public async Task<bool> SignUpAsync(string username, string password, string email)
        {
            return true;
        }
    }
}

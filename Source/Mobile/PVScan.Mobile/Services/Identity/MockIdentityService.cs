using PVScan.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Mobile.Services.Identity
{
    public class MockIdentityService : IIdentityService
    {
        string _token = null;
        public string AccessToken
        {
            get
            {
                return _token;
            }
        }

        public static List<UserInfo> MockUsers = new List<UserInfo>()
        {
            new UserInfo()
            {
                Email = "test1@mail.com",
                Username = "test1",
            },
            new UserInfo()
            {
                Email = "test2@mail.com",
                Username = "test2",
            }
        };

        public static string MockAccessToken = "TEST_TOKEN";

        public async Task Initialize()
        {
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            if (MockUsers.Find(u => u.Username == username) != null)
            {
                _token = MockAccessToken;

                return true;
            }

            return false;
        }

        public async Task<bool> LogoutAsync()
        {
            if (_token == null)
            {
                return false;
            }

            _token = null;
            return true;
        }

        public async Task<bool> SignUpAsync(string username, string password, string email)
        {
            if (MockUsers.Find(u => u.Username == username) != null)
            {
                return false;
            }

            return true;
        }
    }
}

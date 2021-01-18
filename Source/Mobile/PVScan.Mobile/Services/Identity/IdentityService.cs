using IdentityModel.Client;
using Newtonsoft.Json;
using PVScan.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using static IdentityModel.OidcConstants;

namespace PVScan.Mobile.Services.Identity
{
    // Todo: this doesn't belong here
    public static class HttpClientUtils
    {
        public static HttpClient APIHttpClientWithToken(string token)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.BaseAddress = new Uri(IdentityServerConfiguration.API);

            return client;
        }
    }

    public class IdentityService : IIdentityService
    {
        private UserInfo _currentUserInfo;
        public UserInfo CurrentUserInfo
        {
            get
            {
                return _currentUserInfo;
            }
        }

        private string _accessToken;
        public string AccessToken
        {
            get
            {
                return _accessToken;
            }
        }

        public async Task Initialize()
        {
            // Check auth token in storage and check it's validity

            // Todo: change to secure storage
            _accessToken = Preferences.Get("AccessToken", null);

            if (!await ValidateToken(_accessToken))
            {
                // Try to make request for another one
                string prevUsername = Preferences.Get("Username", null);
                string prevPassword = Preferences.Get("Password", null);

                if (!await LoginAsync(prevUsername, prevPassword))
                {
                    // User has to login again
                }
            }
            else
            {
                _currentUserInfo = await GetCurrentUserInfo();

                if (_currentUserInfo == null)
                {
                    // Todo: Whoa!! What an error
                    _accessToken = null;
                }
            }
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            // Login via token endpoint using password flow
            HttpClient httpClient = new HttpClient();

            // Todo: enable https in production
            DiscoveryDocumentResponse discoveryDocument =
                await httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest()
                {
                    Address = IdentityServerConfiguration.Authority,
                    Policy = { RequireHttps = false },
                });

            var token = await httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest()
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = IdentityServerConfiguration.ClientId,
                GrantType = GrantTypes.Password,
                Scope = "openid profile PVScan.API",
                Password = password,
                UserName = username,
            });

            if (token.AccessToken == null)
            {
                return false;
            }

            _accessToken = token.AccessToken;

            _currentUserInfo = await GetCurrentUserInfo();

            if (_currentUserInfo == null)
            {
                // Todo: Whoa!! What an error
                _accessToken = null;
                return false;
            }

            Preferences.Set("AccessToken", _accessToken);
            Preferences.Set("Username", username);
            Preferences.Set("Password", password);

            return true;
        }

        public async Task<bool> LogoutAsync()
        {
            // Logout via logout endpoint and clear local storage
            _accessToken = null;
            _currentUserInfo = null;

            Preferences.Set("AccessToken", null);
            Preferences.Set("Username", null);
            Preferences.Set("Password", null);

            return true;
        }

        public async Task<bool> SignUpAsync(string username, string password, string email)
        {
            // Sign up using register endpoint

            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(IdentityServerConfiguration.Authority);

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "Username", username },
                    { "Password", password },
                    { "Email", email },
                });

            var result = await httpClient.PostAsync("/Auth/Register", content);

            if (!result.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }

        private async Task<UserInfo> GetCurrentUserInfo()
        {
            HttpClient httpClient = HttpClientUtils.APIHttpClientWithToken(_accessToken);
            var result = await httpClient.GetAsync("api/v1/users/current");

            if (!result.IsSuccessStatusCode)
            {
                return null;
            }

            var resultUserInfo = JsonConvert.DeserializeObject<UserInfo>(await result.Content.ReadAsStringAsync());

            return resultUserInfo;
        }

        private async Task<bool> ValidateToken(string token)
        {
            if (String.IsNullOrEmpty(token))
            {
                return false;
            }

            // Make a test request to the backend
            HttpClient httpClient = HttpClientUtils.APIHttpClientWithToken(token);
            var result = await httpClient.GetAsync("api/v1/users/current");

            if (!result.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }
    }
}

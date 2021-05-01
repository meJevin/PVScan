using IdentityModel.Client;
using Newtonsoft.Json;
using PVScan.Mobile.Models;
using PVScan.Mobile.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using static IdentityModel.OidcConstants;

namespace PVScan.Mobile.Services
{
    public class IdentityService : IIdentityService
    {
        readonly IPersistentKVP KVP;
        readonly IHttpClientFactory HttpFactory;

        public IdentityService(IPersistentKVP kvp, IHttpClientFactory httpFactory)
        {
            KVP = kvp;
            HttpFactory = httpFactory;
        }

        private string _accessToken;
        public string AccessToken
        {
            get
            {
                return _accessToken;
            }
        }

        private DiscoveryDocumentResponse DiscoveryDocument = null;

        public async Task Initialize()
        {
            // Check auth token in storage and check it's validity

            // Todo: change to secure storage
            _accessToken = KVP.Get(StorageKeys.AccessToken, null);

            DiscoveryDocument = await GetDiscoveryDocument();

            if (!await ValidateToken(_accessToken))
            {
                _accessToken = null;
                KVP.Set(StorageKeys.AccessToken, null);

                // Try to make request for another one
                string prevUsername = KVP.Get(StorageKeys.Username, null);
                string prevPassword = KVP.Get(StorageKeys.Password, null);

                if (!await LoginAsync(prevUsername, prevPassword))
                {
                    // User has to login again
                }
            }
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password) || DiscoveryDocument == null)
            {
                return false;
            }

            // Login via token endpoint using password flow
            HttpClient httpClient = HttpFactory.Default();

            var token = await httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest()
            {
                Address = DiscoveryDocument.TokenEndpoint,
                ClientId = Auth.ClientId,
                GrantType = GrantTypes.Password,
                Scope = "openid profile PVScan.API",
                Password = password,
                UserName = username,
            }).WithTimeout(DataAccss.WebRequestTimeout);

            if (token == null || token.AccessToken == null)
            {
                return false;
            }

            _accessToken = token.AccessToken;

            KVP.Set(StorageKeys.AccessToken, _accessToken);
            KVP.Set(StorageKeys.Username, username);
            KVP.Set(StorageKeys.Password, password);

            return true;
        }

        public async Task<bool> LogoutAsync()
        {
            if (DiscoveryDocument == null)
            {
                return false;
            }

            //// Logout via logout endpoint and clear local storage
            //HttpClient httpClient = HttpFactory.Default();

            //// Todo: this for some reason can not find token on IS4 auth server but logout still happens
            //var revoke = await httpClient.RevokeTokenAsync(new TokenRevocationRequest()
            //{
            //    Address = DiscoveryDocument.RevocationEndpoint,
            //    ClientId = Auth.ClientId,
            //    Token = _accessToken,
            //    TokenTypeHint = TokenTypes.AccessToken,
            //}).WithTimeout(DataAccss.WebRequestTimeout);

            //if (revoke == null || revoke.IsError)
            //{
            //    return false;
            //}

            HttpClient httpClient = HttpFactory.Default();
            httpClient.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue("Bearer", _accessToken);
            httpClient.BaseAddress = new Uri(Auth.Authority);

            try
            {
                var result = await httpClient
                    .GetAsync("/Auth/Logout")
                    .WithTimeout(DataAccss.WebRequestTimeout);

                if (!result.IsSuccessStatusCode)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                // We need proper error messages!!
                return false;
            }

            _accessToken = null;

            KVP.Set(StorageKeys.AccessToken, null);
            KVP.Set(StorageKeys.Username, null);
            KVP.Set(StorageKeys.Password, null);

            return true;
        }

        public async Task<bool> SignUpAsync(string username, string password, string email)
        {
            // Sign up using register endpoint

            HttpClient httpClient = HttpFactory.Default();
            httpClient.BaseAddress = new Uri(Auth.Authority);

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "Username", username },
                    { "Password", password },
                    { "Email", email },
                });

            try
            {
                var result = await httpClient
                    .PostAsync("/Auth/Register", content)
                    .WithTimeout(DataAccss.WebRequestTimeout);

                if (!result.IsSuccessStatusCode)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                // We need proper error messages!!
                return false;
            }

            return true;
        }

        private async Task<bool> ValidateToken(string token)
        {
            if (String.IsNullOrEmpty(token))
            {
                return false;
            }

            // Make a test request to the backend
            HttpClient httpClient = HttpFactory.ForAPI(token);

            // Todo: This should be done in a different way
            try
            {
                var result = await httpClient
                    .GetAsync("api/v1/users/current")
                    .WithTimeout(DataAccss.WebRequestTimeout);

                if (result == null )
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Todo: add error messages from server...
                return false;
            }

            return true;
        }

        private async Task<DiscoveryDocumentResponse> GetDiscoveryDocument()
        {
            HttpClient httpClient = HttpFactory.Default();

            // Todo: enable https in production
            DiscoveryDocumentResponse discoveryDocument =
                await httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest()
                {
                    Address = Auth.Authority,
                    Policy = { RequireHttps = false },
                }).WithTimeout(DataAccss.WebRequestTimeout);

            if (discoveryDocument == null || discoveryDocument.IsError)
            {
                return null;
            }

            return discoveryDocument;
        }
    }
}

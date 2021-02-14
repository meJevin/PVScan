﻿using IdentityModel.Client;
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
    // Todo: this doesn't belong here
    public static class HttpClientFactory
    {
        private static HttpClientHandler sslIgnoreHandler;
        static HttpClientFactory()
        {
            sslIgnoreHandler = new HttpClientHandler();
            sslIgnoreHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert.Issuer.Equals("CN=localhost"))
                    return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };
        }

        public static HttpClient APIWithToken(string token)
        {
            HttpClient client = Default();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.BaseAddress = new Uri(API.BaseAddress);

            return client;
        }

        public static HttpClient Default()
        {
#if DEBUG
            return new HttpClient(sslIgnoreHandler);
#else
            return new HttpClient();
#endif
        }
    }

    public class IdentityService : IIdentityService
    {
        readonly IPersistentKVP KVP;

        public IdentityService(IPersistentKVP kvp)
        {
            KVP = kvp;
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
            _accessToken = KVP.Get("AccessToken", null);

            if (!await ValidateToken(_accessToken))
            {
                _accessToken = null;
                KVP.Set("AccessToken", null);

                // Try to make request for another one
                string prevUsername = KVP.Get("Username", null);
                string prevPassword = KVP.Get("Password", null);

                if (!await LoginAsync(prevUsername, prevPassword))
                {
                    // User has to login again
                }
            }
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
            {
                return false;
            }

            // Login via token endpoint using password flow
            HttpClient httpClient = HttpClientFactory.Default();

            // Todo: enable https in production
            DiscoveryDocumentResponse discoveryDocument =
                await httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest()
                {
                    Address = Auth.Authority,
                    Policy = { RequireHttps = false },
                });

            var token = await httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest()
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = Auth.ClientId,
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

            KVP.Set("AccessToken", _accessToken);
            KVP.Set("Username", username);
            KVP.Set("Password", password);

            return true;
        }

        public async Task<bool> LogoutAsync()
        {
            // Logout via logout endpoint and clear local storage
            HttpClient httpClient = HttpClientFactory.Default();

            DiscoveryDocumentResponse discoveryDocument =
                await httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest()
                {
                    Address = Auth.Authority,
                    Policy = { RequireHttps = false },
                });

            // Todo: this for some reason can not find token on IS4 auth server but logout still happens
            var revoke = await httpClient.RevokeTokenAsync(new TokenRevocationRequest()
            {
                Address = discoveryDocument.RevocationEndpoint,
                ClientId = Auth.ClientId,
                Token = _accessToken,
                TokenTypeHint = TokenTypes.AccessToken,
            });

            if (revoke.IsError)
            {
                return false;
            }

            _accessToken = null;

            KVP.Set("AccessToken", null);
            KVP.Set("Username", null);
            KVP.Set("Password", null);

            return true;
        }

        public async Task<bool> SignUpAsync(string username, string password, string email)
        {
            // Sign up using register endpoint

            HttpClient httpClient = HttpClientFactory.Default();
            httpClient.BaseAddress = new Uri(Auth.Authority);

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

        private async Task<bool> ValidateToken(string token)
        {
            if (String.IsNullOrEmpty(token))
            {
                return false;
            }

            // Make a test request to the backend
            HttpClient httpClient = HttpClientFactory.APIWithToken(token);
            var result = await httpClient.GetAsync("api/v1/users/current");

            if (!result.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }
    }
}

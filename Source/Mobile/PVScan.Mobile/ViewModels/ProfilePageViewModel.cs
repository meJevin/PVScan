using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using MvvmHelpers;
using PVScan.Mobile.Services.Identity;
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
        OidcClient _client;

        public ProfilePageViewModel()
        {
            var browser = DependencyService.Get<IBrowser>();
            var options = new OidcClientOptions
            {
                Authority = IdentityServerConfiguration.Authority,
                ClientId = IdentityServerConfiguration.ClientId,
                Scope = "openid profile PVScan.API",
                RedirectUri = IdentityServerConfiguration.RedirectUri,
                Browser = new Browser(),

                ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect,
            };
            // Todo: change to production later
            options.Policy.Discovery.RequireHttps = false;

            _client = new OidcClient(options);

            LoginCommand = new Command(async () =>
            {
                try
                {
                    var _result = await _client.LoginAsync(new LoginRequest() { BrowserDisplayMode = DisplayMode.Hidden });

                    if (_result.IsError)
                    {
                        return;
                    }

                    var _httpClient = new HttpClient();
                    _httpClient.BaseAddress = new Uri("http://10.0.2.2:1337");
                    _httpClient.DefaultRequestHeaders.Authorization
                        = new AuthenticationHeaderValue("Bearer", _result?.AccessToken ?? string.Empty);

                    HttpResponseMessage response = await _httpClient.GetAsync("api/v1/users/current");
                    string content = await response.Content.ReadAsStringAsync();

                    Console.WriteLine("Hi, your name is: " + _result.User.Identity.Name);
                }
                catch (Exception ex)
                {
                }
            });
        }

        public ICommand LoginCommand { get; }
    }
    public class Browser : IBrowser
    {
        public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
        {
            WebAuthenticatorResult authResult =
                    await WebAuthenticator.AuthenticateAsync(new Uri(options.StartUrl), new Uri(IdentityServerConfiguration.RedirectUri));

            return new BrowserResult()
            {
                Response = ParseAuthenticatorResult(authResult)
            };
        }

        string ParseAuthenticatorResult(WebAuthenticatorResult result)
        {
            string code = result?.Properties["code"];
            string scope = result?.Properties["scope"];
            string state = result?.Properties["state"];
            string sessionState = result?.Properties["session_state"];
            return $"{IdentityServerConfiguration.RedirectUri}#code={code}&scope={scope}&state={state}&session_state={sessionState}";
        }
    }
}

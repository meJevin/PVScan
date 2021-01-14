using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using MvvmHelpers;
using PVScan.Mobile.Services.Identity;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
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
                Browser = browser,

                ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect,
            };
            // Todo: change to production later
            options.Policy.Discovery.RequireHttps = false;

            _client = new OidcClient(options);

            LoginCommand = new Command(async () =>
            {
                try
                {
                    var _result = await _client.LoginAsync(new LoginRequest());

                    if (_result.IsError)
                    {
                        return;
                    }

                    Console.WriteLine("Hi, your name is: " + _result.User.Identity.Name);
                }
                catch (Exception ex)
                {
                }
            });
        }

        public ICommand LoginCommand { get; }
    }
}

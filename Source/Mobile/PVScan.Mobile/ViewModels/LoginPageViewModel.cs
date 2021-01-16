using IdentityModel.Client;
using MvvmHelpers;
using PVScan.Mobile.Services.Identity;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using static IdentityModel.OidcConstants;

namespace PVScan.Mobile.ViewModels
{
    public class LoginPageViewModel : BaseViewModel
    {
        public LoginPageViewModel()
        {
            LoginCommand = new Command(async () =>
            {
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
                    ClientId = "PVScan.Auth.Mobile",
                    GrantType = GrantTypes.Password,
                    Scope = "openid profile PVScan.API",
                    Password = Password,
                    UserName = Login,
                });

                if (token.AccessToken != null)
                {
                    // Success
                }
                else
                {

                }
            });
        }

        public ICommand LoginCommand { get; }

        public string Login { get; set; }
        public string Password { get; set; }
    }
}

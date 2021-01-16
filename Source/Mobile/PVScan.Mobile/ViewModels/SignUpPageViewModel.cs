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
    public class SignUpPageViewModel : BaseViewModel
    {
        public SignUpPageViewModel()
        {
            SignUpCommand = new Command(async () =>
            {
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(IdentityServerConfiguration.Authority);

                var content = new FormUrlEncodedContent(new Dictionary<string, string> 
                {
                    { "Username", Login },
                    { "Password", Password },
                    { "Email", Email },
                });

                var result = await httpClient.PostAsync("/Auth/Register", content);

                if (result.IsSuccessStatusCode 
                    || result.StatusCode == System.Net.HttpStatusCode.Redirect)
                {
                    // Success
                }
                else
                {
                    // Error
                }
            });
        }

        public ICommand SignUpCommand { get;  }

        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

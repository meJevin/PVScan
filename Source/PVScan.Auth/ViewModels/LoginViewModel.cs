using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;

namespace PVScan.Auth.ViewModels
{
    public class LoginViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
        public IEnumerable<AuthenticationScheme> ExternalProviders { get; set; }
    }
}
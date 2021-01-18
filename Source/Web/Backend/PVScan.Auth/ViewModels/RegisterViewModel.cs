using System.ComponentModel.DataAnnotations;

namespace PVScan.Auth.ViewModels
{
    public class RegisterViewModel
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}
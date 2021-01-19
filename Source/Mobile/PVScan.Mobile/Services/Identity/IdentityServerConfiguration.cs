using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PVScan.Mobile.Services.Identity
{
    public static class IdentityServerConfiguration
    {
        public static string Authority = "http://192.168.1.74:1488";
        public const string RedirectUri = "pvscan://callback";
        public const string ClientId = "PVScan.Auth.Mobile";

        // Todo: this doesn't belong here
        public static string API = "http://192.168.1.74:1337";
    }
}

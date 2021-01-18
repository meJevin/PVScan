using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PVScan.Mobile.Services.Identity
{
    public static class IdentityServerConfiguration
    {
        public static string Authority = Device.RuntimePlatform == Device.Android ? "http://10.0.2.2:1488" : "http://localhost:1488";
        public const string RedirectUri = "pvscan://callback";
        public const string ClientId = "PVScan.Auth.Mobile";

        // Todo: this doesn't belong here
        public static string API = Device.RuntimePlatform == Device.Android? "http://10.0.2.2:1337" : "http://localhost:1337";
    }
}

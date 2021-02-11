using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PVScan.Mobile
{
    public static class Auth
    {
        public static string Authority = "http://192.168.1.74:1488";
        public const string ClientId = "PVScan.Auth.Mobile";
        //public const string RedirectUri = "pvscan://callback"; // Code flow auth
    }

    public static class API
    {
        // Todo: this doesn't belong here
        public static string BaseAddress = "http://192.168.1.74:1337";
    }
}

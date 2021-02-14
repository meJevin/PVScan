using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PVScan.Mobile
{
    public static class Auth
    {
        public static string Authority = DeviceInfo.Platform == DevicePlatform.Android ? "https://10.0.2.2:1488" : "https://localhost:1488";
        public const string ClientId = "PVScan.Auth.Mobile";
        //public const string RedirectUri = "pvscan://callback"; // Code flow auth
    }

    public static class API
    {
        // Todo: this doesn't belong here
        public static string BaseAddress = DeviceInfo.Platform == DevicePlatform.Android ? "https://10.0.2.2:1337" : "https://localhost:1337";
    }

    public static class DataAccss
    {
        public static string DatabasePath
        {
            get
            {
                return System.IO.Path.Combine(FileSystem.AppDataDirectory, "PVScan.db3");
            }
        }
    }


    public static class StorageKeys
    {
        public static string AccessToken = "AccessToken";

        public static string Username = "Username";
        public static string Password = "Password";

        public static string Theme = "Theme";
    }
}

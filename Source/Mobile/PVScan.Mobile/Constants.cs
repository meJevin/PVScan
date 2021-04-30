using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PVScan.Mobile
{
    public static class Auth
    {
        public static string Authority = 
            DeviceInfo.Platform == DevicePlatform.Android ? "https://10.0.2.2:1488" : "https://localhost:1488";
        public const string ClientId = "PVScan.Auth.Mobile";
    }

    public static class API
    {
        public static string BaseAddress = 
            DeviceInfo.Platform == DevicePlatform.Android ? "https://10.0.2.2:1337" : "https://localhost:1337";
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

        public static TimeSpan WebRequestTimeout = TimeSpan.FromSeconds(5);
    }

    public static class StorageKeys
    {
        public static string AccessToken = nameof(AccessToken);

        public static string Username = nameof(Username);
        public static string Password = nameof(Password);

        public static string Theme = nameof(Theme);
        public static string SaveBarcodeImagesWithAlpha = nameof(SaveBarcodeImagesWithAlpha);

        public static class Defaults
        {
            public static string AccessToken = null;

            public static string Username = null;
            public static string Password = null;

            public static string Theme = null;
            public static bool SaveBarcodeImagesWithAlpha = false;
        }
    }
}

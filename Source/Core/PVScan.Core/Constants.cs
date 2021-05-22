using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PVScan.Core
{
    public static class Auth
    {
#if MOBILE_ANDROID
        public static string Authority = "https://10.0.2.2:1488";
#elif MOBILE_IOS
        public static string Authority = "https://192.168.1.23:1488";
#elif DESKTOP_WINDOWS
        public static string Authority = "https://localhost:1488";
#endif

#if MOBILE_ANDROID || MOBILE_IOS
        public const string ClientId = "PVScan.Auth.Mobile";
#elif DESKTOP_WINDOWS
        public const string ClientId = "PVScan.Auth.Desktop";
#endif
    }

    public static class API
    {

#if MOBILE_ANDROID
        public static string BaseAddress = "https://10.0.2.2:1337";
#elif MOBILE_IOS
        public static string BaseAddress = "https://192.168.1.23:1337";
#elif DESKTOP_WINDOWS
        public static string BaseAddress = "https://localhost:1337";
#endif
        public static string BarcodesHub = "/hubs/barcodes";
        public static string UserInfoHub = "/hubs/userInfo";
    }

    public static class DataAccess
    {
        public static void Init(string DBPath)
        {
            DatabasePath = DBPath;
        }

        public static string DatabasePath 
        {
            get;
            private set;
        }

        public static TimeSpan WebRequestTimeout = TimeSpan.FromSeconds(10);
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

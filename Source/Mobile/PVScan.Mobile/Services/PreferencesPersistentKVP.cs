using PVScan.Core.Services.Interfaces;
using PVScan.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace PVScan.Mobile.Services
{
    public class PreferencesPersistentKVP : IPersistentKVP
    {
        public async Task Clear()
        {
            Preferences.Clear();
        }

        public async Task<bool> ContainsKey(string key)
        {
            return Preferences.ContainsKey(key);
        }

        public async Task<string> Get(string key, string defaultValue)
        {
            return Preferences.Get(key, defaultValue);
        }

        public async Task<bool> Get(string key, bool defaultValue)
        {
            return Preferences.Get(key, defaultValue);
        }

        public async Task Remove(string key)
        {
            Preferences.Remove(key);
        }

        public async Task Set(string key, string value)
        {
            Preferences.Set(key, value);
        }

        public async Task Set(string key, bool value)
        {
            Preferences.Set(key, value);
        }
    }
}

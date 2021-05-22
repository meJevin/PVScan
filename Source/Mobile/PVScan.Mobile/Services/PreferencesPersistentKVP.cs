using PVScan.Core.Services.Interfaces;
using PVScan.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace PVScan.Mobile.Services
{
    public class PreferencesPersistentKVP : IPersistentKVP
    {
        public void Clear()
        {
            Preferences.Clear();
        }

        public bool ContainsKey(string key)
        {
            return Preferences.ContainsKey(key);
        }

        public string Get(string key, string defaultValue)
        {
            return Preferences.Get(key, defaultValue);
        }

        public bool Get(string key, bool defaultValue)
        {
            return Preferences.Get(key, defaultValue);
        }

        public void Remove(string key)
        {
            Preferences.Remove(key);
        }

        public void Set(string key, string value)
        {
            Preferences.Set(key, value);
        }

        public void Set(string key, bool value)
        {
            Preferences.Set(key, value);
        }
    }
}

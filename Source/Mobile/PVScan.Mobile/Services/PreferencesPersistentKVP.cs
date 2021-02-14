using PVScan.Mobile.Services.Interfaces;
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

        public dynamic Get(string key, dynamic defaultValue)
        {
            return Preferences.Get(key, defaultValue);
        }

        public void Remove(string key)
        {
            Preferences.Remove(key);
        }

        public void Set(string key, dynamic value)
        {
            Preferences.Set(key, value);
        }
    }
}

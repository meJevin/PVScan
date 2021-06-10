using PVScan.Core.Services.Interfaces;
using PVScan.Mobile.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PVScan.Mobile.Tests.Services.Mocks
{
    public class InMemoryKVP : IPersistentKVP
    {
        Dictionary<string, object> KVPs;

        public InMemoryKVP()
        {
            KVPs = new Dictionary<string, object>();
        }

        public void Clear()
        {
            KVPs.Clear();
        }

        public bool ContainsKey(string key)
        {
            return KVPs.ContainsKey(key);
        }

        public string Get(string key, string defaultValue)
        {
            if (!KVPs.TryGetValue(key, out object result))
            {
                return defaultValue;
            }

            if (!(result is string))
            {
                return defaultValue;
            }

            return (string)result;
        }

        public bool Get(string key, bool defaultValue)
        {
            if (!KVPs.TryGetValue(key, out object result))
            {
                return defaultValue;
            }

            if (!(result is bool))
            {
                return defaultValue;
            }

            return (bool)result;
        }

        public void Remove(string key)
        {
            KVPs.Remove(key);
        }

        public void Set(string key, string value)
        {
            KVPs[key] = value;
        }

        public void Set(string key, bool value)
        {
            KVPs[key] = value;
        }
    }

}

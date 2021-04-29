using PVScan.Mobile.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PVScan.Mobile.Tests.Services.Mocks
{
    public class InMemoryKVP : IPersistentKVP
    {
        Dictionary<string, string> KVPs;

        public InMemoryKVP()
        {
            KVPs = new Dictionary<string, string>();
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
            if (!KVPs.TryGetValue(key, out string result))
            {
                return null;
            }

            return result;
        }

        public void Remove(string key)
        {
            KVPs.Remove(key);
        }

        public void Set(string key, string value)
        {
            KVPs[key] = value;
        }
    }

}

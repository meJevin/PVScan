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

        public dynamic Get(string key, dynamic defaultValue)
        {
            if (!KVPs.TryGetValue(key, out object result))
            {
                return null;
            }

            return result;
        }

        public void Remove(string key)
        {
            KVPs.Remove(key);
        }

        public void Set(string key, dynamic value)
        {
            KVPs[key] = value;
        }
    }

}

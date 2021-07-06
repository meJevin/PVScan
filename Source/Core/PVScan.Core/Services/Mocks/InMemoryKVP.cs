using PVScan.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Core.Services.Mocks
{
    public class InMemoryKVP : IPersistentKVP
    {
        Dictionary<string, object> KVPs;

        public InMemoryKVP()
        {
            KVPs = new Dictionary<string, object>();
        }

        public async Task Clear()
        {
            KVPs.Clear();
        }

        public async Task<bool> ContainsKey(string key)
        {
            return KVPs.ContainsKey(key);
        }

        public async Task<string> Get(string key, string defaultValue)
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

        public async Task<bool> Get(string key, bool defaultValue)
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

        public async Task Remove(string key)
        {
            KVPs.Remove(key);
        }

        public async Task Set(string key, string value)
        {
            KVPs[key] = value;
        }

        public async Task Set(string key, bool value)
        {
            KVPs[key] = value;
        }
    }

}

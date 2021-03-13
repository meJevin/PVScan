using System;
using System.Collections.Generic;
using System.Text;

namespace PVScan.Mobile.Services.Interfaces
{
    public interface IPersistentKVP
    {
        void Clear();

        bool ContainsKey(string key);

        string Get(string key, string defaultValue);

        void Remove(string key);

        void Set(string key, string value);
    }
}

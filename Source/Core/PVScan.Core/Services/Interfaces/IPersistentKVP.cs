using System;
using System.Collections.Generic;
using System.Text;

namespace PVScan.Core.Services.Interfaces
{
    public interface IPersistentKVP
    {
        void Clear();

        bool ContainsKey(string key);

        string Get(string key, string defaultValue);
        bool Get(string key, bool defaultValue);

        void Remove(string key);

        void Set(string key, string value);
        void Set(string key, bool value);
    }
}

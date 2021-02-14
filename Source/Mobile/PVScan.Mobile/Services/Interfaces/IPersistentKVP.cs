using System;
using System.Collections.Generic;
using System.Text;

namespace PVScan.Mobile.Services.Interfaces
{
    public interface IPersistentKVP
    {
        void Clear();

        bool ContainsKey(string key);

        dynamic Get(string key, dynamic defaultValue);

        void Remove(string key);

        void Set(string key, dynamic value);
    }
}

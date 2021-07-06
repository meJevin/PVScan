using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Core.Services.Interfaces
{
    public interface IPersistentKVP
    {
        Task Clear();

        Task<bool> ContainsKey(string key);

        Task<string> Get(string key, string defaultValue);
        Task<bool> Get(string key, bool defaultValue);

        Task Remove(string key);

        Task Set(string key, string value);
        Task Set(string key, bool value);
    }
}

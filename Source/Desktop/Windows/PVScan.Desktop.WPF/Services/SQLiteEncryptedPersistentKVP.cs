using Microsoft.EntityFrameworkCore;
using PVScan.Core.Services.Interfaces;
using PVScan.Desktop.WPF.Models;
using PVScan.Desktop.WPF.Services.SQLiteEncrypted;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Desktop.WPF.Services
{
    public class SQLiteEncryptedPersistentKVP : IPersistentKVP
    {
        readonly SQLiteEncryptedDbContext _context;

        public SQLiteEncryptedPersistentKVP(SQLiteEncryptedDbContext dbContext)
        {
            _context = dbContext;
        }


        public async Task Clear()
        {
            await Task.Run(async () =>
            {
                _context.KVPs.RemoveRange(_context.KVPs.ToList());
                await _context.SaveChangesAsync();
            });
        }

        public async Task<bool> ContainsKey(string key)
        {
            return await Task.Run(async () =>
            {
                return (await _context.KVPs.Where(kvp => kvp.Key == key).FirstOrDefaultAsync()) != null;
            });
        }

        public async Task<string> Get(string key, string defaultValue)
        {
            return await Task.Run(async () =>
            {
                var dbKVP = await _context.KVPs.Where(kvp => kvp.Key == key).FirstOrDefaultAsync();

                if (dbKVP == null || dbKVP.Type != typeof(string).Name)
                {
                    return defaultValue;
                }

                return dbKVP.Value;
            });
        }

        public async Task<bool> Get(string key, bool defaultValue)
        {
            return await Task.Run(async () =>
            {
                var dbKVP = await _context.KVPs.Where(kvp => kvp.Key == key).FirstOrDefaultAsync();

                if (dbKVP == null || dbKVP.Type != typeof(string).Name)
                {
                    return defaultValue;
                }

                return bool.Parse(dbKVP.Value);
            });
        }

        public async Task Remove(string key)
        {
            await Task.Run(async () =>
            {
                var dbKVP = await _context.KVPs.Where(kvp => kvp.Key == key).FirstOrDefaultAsync();

                if (dbKVP == null)
                {
                    return;
                }

                _context.KVPs.Remove(dbKVP);
                await _context.SaveChangesAsync();
            });
        }

        public async Task Set(string key, string value)
        {
            await Task.Run(async () =>
            {
                var dbKVP = await _context.KVPs.Where(kvp => kvp.Key == key).FirstOrDefaultAsync();

                if (dbKVP == null || dbKVP.Type != typeof(string).Name)
                {
                    _context.KVPs.Add(new SQLiteEncrypedKVP()
                    {
                        Key = key,
                        Value = value,
                        Type = typeof(string).Name,
                    });
                }
                else
                {
                    dbKVP.Value = value;
                }

                await _context.SaveChangesAsync();
            });
        }

        public async Task Set(string key, bool value)
        {
            await Task.Run(async () =>
            {
                var dbKVP = await _context.KVPs.Where(kvp => kvp.Key == key).FirstOrDefaultAsync();

                if (dbKVP == null || dbKVP.Type != typeof(string).Name)
                {
                    _context.KVPs.Add(new SQLiteEncrypedKVP()
                    {
                        Key = key,
                        Value = value.ToString(),
                        Type = typeof(bool).Name,
                    });
                }
                else
                {
                    dbKVP.Value = value.ToString();
                }

                await _context.SaveChangesAsync();
            });
        }
    }
}

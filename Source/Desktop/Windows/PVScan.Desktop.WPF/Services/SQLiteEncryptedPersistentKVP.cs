using Microsoft.EntityFrameworkCore;
using PVScan.Core.Services.Interfaces;
using PVScan.Desktop.WPF.Models;
using PVScan.Desktop.WPF.Services.SQLiteEncrypted;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PVScan.Desktop.WPF.Services
{
    public class SQLiteEncryptedPersistentKVP : IPersistentKVP
    {
        readonly SQLiteEncryptedDbContext _context;

        public SQLiteEncryptedPersistentKVP(SQLiteEncryptedDbContext dbContext)
        {
            _context = dbContext;
        }


        public void Clear()
        {
            _context.KVPs.RemoveRange(_context.KVPs.ToList());
            _context.SaveChanges();
        }

        public bool ContainsKey(string key)
        {
            return _context.KVPs.Where(kvp => kvp.Key == key).FirstOrDefault() != null;
        }

        public string Get(string key, string defaultValue)
        {
            var dbKVP = _context.KVPs.Where(kvp => kvp.Key == key).FirstOrDefault();

            if (dbKVP == null || dbKVP.Type != typeof(string).Name)
            {
                return defaultValue;
            }

            return dbKVP.Value;
        }

        public bool Get(string key, bool defaultValue)
        {
            var dbKVP = _context.KVPs.Where(kvp => kvp.Key == key).FirstOrDefault();

            if (dbKVP == null || dbKVP.Type != typeof(string).Name)
            {
                return defaultValue;
            }

            return bool.Parse(dbKVP.Value);
        }

        public void Remove(string key)
        {
            var dbKVP = _context.KVPs.Where(kvp => kvp.Key == key).FirstOrDefault();

            if (dbKVP == null)
            {
                return;
            }

            _context.KVPs.Remove(dbKVP);
            _context.SaveChanges();
        }

        public void Set(string key, string value)
        {
            var dbKVP = _context.KVPs.Where(kvp => kvp.Key == key).FirstOrDefault();

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

            _context.SaveChanges();
        }

        public void Set(string key, bool value)
        {
            var dbKVP = _context.KVPs.Where(kvp => kvp.Key == key).FirstOrDefault();

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

            _context.SaveChanges();
        }
    }
}

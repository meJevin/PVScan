using Autofac;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PVScan.Desktop.WPF.DI;
using PVScan.Desktop.WPF.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace PVScan.Desktop.WPF.Services.SQLiteEncrypted
{
    public class SQLiteEncryptedDbContext : DbContext
    {
        public SQLiteEncryptedDbContext() : base()
        {
        }

        public SQLiteEncryptedDbContext(DbContextOptions<SQLiteEncryptedDbContext> options)
            : base(options)
        {
            Database.OpenConnection();

            if (Database.GetPendingMigrations().Any())
            {
                Database.Migrate();
            }
        }

        // This is only needed to make migrations :)
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(GetConnection(Constants.SQLiteEncryptedKVPDatabasePath));
        }

        public static SqliteConnection GetConnection(string dbFilePath)
        {
            using var scope = Resolver.Container.BeginLifetimeScope();
            var cfg = scope.Resolve<IConfiguration>();

#if DEBUG
            var key = cfg.GetSection("SQLiteEncryptedKey_Debug").Value;
#else
            var key = cfg.GetSection("SQLiteEncryptedKey").Value;
#endif

            var connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = dbFilePath,
                Password = key,
            };

            return new SqliteConnection(connectionString.ToString());
        }

        public override void Dispose()
        {
            base.Dispose();

            Database.CloseConnection();
        }

        public DbSet<SQLiteEncrypedKVP> KVPs { get; set; }
    }

}

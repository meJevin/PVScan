using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
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
        // This is only needed to make migrations :)
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(GetConnection(Constants.SQLiteEncryptedKVPDatabasePath));
        }

        public static SqliteConnection GetConnection(string dbFilePath)
        {
            var connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = dbFilePath,
                Password = "Test123",
            };
            return new SqliteConnection(connectionString.ToString());
        }

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

        public override void Dispose()
        {
            base.Dispose();

            Database.CloseConnection();
        }

        public DbSet<SQLiteEncrypedKVP> KVPs { get; set; }
    }

}

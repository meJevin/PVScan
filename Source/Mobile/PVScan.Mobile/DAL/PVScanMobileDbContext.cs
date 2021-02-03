using Microsoft.EntityFrameworkCore;
using PVScan.Mobile.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PVScan.Mobile.DAL
{
    public class PVScanMobileDbContext : DbContext
    {
        readonly string dbPath;

        public PVScanMobileDbContext()
        {
        }

        public PVScanMobileDbContext(string dbPath)
        {
            this.dbPath = dbPath;

            SQLitePCL.Batteries_V2.Init();

            Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Barcode>().OwnsOne(b => b.ScanLocation);
        }

        public DbSet<Barcode> Barcodes { get; set; }
    }
}

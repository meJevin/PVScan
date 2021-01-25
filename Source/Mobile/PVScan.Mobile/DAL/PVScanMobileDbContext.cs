using Microsoft.EntityFrameworkCore;
using PVScan.Mobile.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Essentials;

namespace PVScan.Mobile.DAL
{
    public class PVScanMobileDbContext : DbContext
    {
        public PVScanMobileDbContext()
        {
            SQLitePCL.Batteries_V2.Init();

            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "PVScan.db3");

            optionsBuilder.UseSqlite($"Filename={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Barcode>().OwnsOne(b => b.ScanLocation);
        }

        public DbSet<Barcode> Barcodes { get; set; }
    }
}

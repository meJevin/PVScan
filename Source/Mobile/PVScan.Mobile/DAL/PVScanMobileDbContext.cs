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
        public PVScanMobileDbContext(DbContextOptions<PVScanMobileDbContext> options) 
            : base(options)
        {
            if (Database.IsSqlite())
            {
                SQLitePCL.Batteries_V2.Init();
                Database.Migrate();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Barcode>().OwnsOne(b => b.ScanLocation);
        }

        public DbSet<Barcode> Barcodes { get; set; }
    }
}

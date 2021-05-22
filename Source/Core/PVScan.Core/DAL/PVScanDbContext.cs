using Microsoft.EntityFrameworkCore;
using PVScan.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PVScan.Core.DAL
{
    public class PVScanDbContext : DbContext
    {
        // This is only needed to make migrations :)
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlite();
        //}

        public PVScanDbContext() : base()
        {

        }

        public PVScanDbContext(DbContextOptions<PVScanDbContext> options) 
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

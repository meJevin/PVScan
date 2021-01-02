using Microsoft.EntityFrameworkCore;
using PVScan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PVScan.EntityFramework
{
    public class PVScanDbContext : DbContext
    {
        public PVScanDbContext(DbContextOptions<PVScanDbContext> options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Primary key as email
            modelBuilder.Entity<User>().HasKey(u => u.Email);

            modelBuilder.Entity<Barcode>().HasKey(b => b.Id);
            modelBuilder.Entity<Barcode>().HasOne(b => b.ScannedBy).WithMany(u => u.Barcodes);
        }

        public DbSet<Barcode> Barcodes { get; set; }
        public DbSet<User> Users { get; set; }
    }
}

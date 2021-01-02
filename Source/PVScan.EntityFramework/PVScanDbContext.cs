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
            modelBuilder.Entity<Barcode>().OwnsOne(b => b.Location);
        }

        public DbSet<Barcode> Barcodes { get; set; }
        public DbSet<User> Users { get; set; }
    }

    public static class PVScanDbContextFactory
    {
        public static PVScanDbContext Create(Action<DbContextOptionsBuilder<PVScanDbContext>> config)
        {
            var builder = new DbContextOptionsBuilder<PVScanDbContext>();

            config(builder);

            var options = builder.Options;

            return new PVScanDbContext(options);
        }
    }
}

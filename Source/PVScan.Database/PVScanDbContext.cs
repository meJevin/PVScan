using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PVScan.Database.Identity;
using PVScan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PVScan.Database
{
    public class PVScanDbContext : IdentityDbContext<ApplicationUser>
    {
        public PVScanDbContext(DbContextOptions<PVScanDbContext> options) :
            base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Barcode>().OwnsOne(b => b.ScanLocation);
        }

        public DbSet<Barcode> Barcodes { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }
    }
}

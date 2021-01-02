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

        public DbSet<Barcode> Barcodes { get; set; }
        public DbSet<User> Users { get; set; }
    }
}

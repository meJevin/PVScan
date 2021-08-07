using Microsoft.EntityFrameworkCore;
using PVScan.Core.DAL;
using PVScan.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PVScan.Core.Services
{
    public class PVScanDbContextFactory : IPVScanDbContextFactory
    {
        public PVScanDbContext Get()
        {
            var optionsBuilder = new DbContextOptionsBuilder<PVScanDbContext>();

            optionsBuilder.UseSqlite(
                $"Filename={DataAccess.DatabasePath}",
                sqliteOptions =>
                {
                    var migrationsAssembly = typeof(PVScanDbContext).GetTypeInfo().Assembly.GetName().Name;
                });

            return new PVScanDbContext(optionsBuilder.Options);
        }
    }
}

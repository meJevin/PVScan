using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PVScan.Database.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPVScanDatabase(this IServiceCollection services,
            string connectionString)
        {
            var migrationsAssembly = typeof(PVScanDbContext).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<PVScanDbContext>(options =>
            {
                options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
            });

            return services;
        }
    }
}

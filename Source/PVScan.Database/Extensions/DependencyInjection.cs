using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PVScan.Database.Identity;
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

        public static IServiceCollection ConfigureIdentityServer(this IServiceCollection services,
            string connectionString)
        {
            var migrationAssembly = typeof(PVScanDbContext).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                config.Password.RequiredLength = 4;
                config.Password.RequireDigit = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<PVScanDbContext>()
                .AddDefaultTokenProviders();

            services.AddIdentityServer()
                .AddAspNetIdentity<ApplicationUser>()
                .AddConfigurationStore(cfg =>
                {
                    cfg.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString, sql =>
                            sql.MigrationsAssembly(migrationAssembly));
                })
                .AddOperationalStore(cfg =>
                {
                    cfg.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString, sql =>
                            sql.MigrationsAssembly(migrationAssembly));

                    cfg.EnableTokenCleanup = true;
                    cfg.TokenCleanupInterval = 30;
                })
                .AddDeveloperSigningCredential();

            return services;
        }
    }
}

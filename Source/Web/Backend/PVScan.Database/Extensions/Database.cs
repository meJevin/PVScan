using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Database.Extensions
{
    public static class Database
    {
        public static IApplicationBuilder EnsurePVScanDatabaseTablesCreated(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var contextMain = scope.ServiceProvider.GetRequiredService<PVScanDbContext>();

            if (contextMain.Database.IsSqlServer())
            {
                contextMain.Database.Migrate();
            }

            return app;
        }
        public static IApplicationBuilder EnsureIdentityServerDatabaseTablesCreated(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var contextPersistedGrant = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
            var contextConfiguration = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            if (contextPersistedGrant.Database.IsSqlServer()
                && contextConfiguration.Database.IsSqlServer())
            {
                contextPersistedGrant.Database.Migrate();
                contextConfiguration.Database.Migrate();
            }

            return app;
        }
    }
}

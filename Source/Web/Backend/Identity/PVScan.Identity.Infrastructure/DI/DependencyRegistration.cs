using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PVScan.Identity.Domain.Entities;
using PVScan.Identity.Infrastructure.Configurations;
using PVScan.Identity.Infrastructure.Data;
using PVScan.Identity.Infrastructure.Data.Repositories;
using PVScan.Identity.Infrastructure.Data.Repositories.Interfaces;
using PVScan.Identity.Infrastructure.Services;
using PVScan.Shared.Configurations;
using PVScan.Shared.Data;
using PVScan.Shared.DI;
using PVScan.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Identity.Infrastructure.DI
{
    public static class DependencyRegistration
    {
        public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IJwtTokenFactory, JwtTokenFactory>();

            services.AddRepositoriesFrom(typeof(PVScanIdentityDbContext).Assembly);

            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            services.Configure<IdentitySettings>(configuration.GetSection(nameof(IdentitySettings)));

            services.AddIdentityDbContext(configuration);

            services
                .AddIdentityCore<User>()
                .AddRoles<IdentityRole<Guid>>()
                .AddEntityFrameworkStores<PVScanIdentityDbContext>()
                .AddDefaultTokenProviders()
                .AddRoleManager<RoleManager<IdentityRole<Guid>>>()
                .AddSignInManager<SignInManager<User>>();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return services;
        }

        private static IServiceCollection AddIdentityDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PVScanIdentityDbContext>(dbContextCfg =>
            {
                var postgresSettings = new PostgresSettings();
                configuration.GetSection(nameof(PostgresSettings)).Bind(postgresSettings);

                dbContextCfg.UseNpgsql(postgresSettings.ConnectionString, pgSqlCfg =>
                {
                    // Optionally configure PostgreSQL...
                });
            });

            return services;
        }
    }
}

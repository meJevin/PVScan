using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PVScan.Database;
using PVScan.Database.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PVScan.Auth
{
    public class Startup
    {
        IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureDbContext(services);

            services.AddControllersWithViews();

            services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "PVScan.Auth.Cookie";
                config.LoginPath = "/Auth/Login";
                config.LogoutPath = "/Auth/Logout";
            });

            ConfigureIdentityServer(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            InitializeDatabase(app);

            app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        private void ConfigureDbContext(IServiceCollection services)
        {
            var connectionString = Configuration["DBConnection"];
            var migrationsAssembly = typeof(PVScanDbContext).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<PVScanDbContext>(options =>
            {
                options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
            });
        }

        private void ConfigureIdentityServer(IServiceCollection services)
        {
            var connectionString = Configuration["DBConnection"];
            var migrationsAssemblyIS4 = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentityServer()
                .AddAspNetIdentity<ApplicationUser>()
                .AddConfigurationStore(cfg =>
                {
                    cfg.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString, sql =>
                            sql.MigrationsAssembly(migrationsAssemblyIS4));
                })
                .AddOperationalStore(cfg =>
                {
                    cfg.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString, sql =>
                            sql.MigrationsAssembly(migrationsAssemblyIS4));

                    cfg.EnableTokenCleanup = true;
                    cfg.TokenCleanupInterval = 30;
                })
                .AddDeveloperSigningCredential();

            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                config.Password.RequiredLength = 4;
                config.Password.RequireDigit = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<PVScanDbContext>()
                .AddDefaultTokenProviders();
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                serviceScope.ServiceProvider.GetRequiredService<PVScanDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.Clients)
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.IdentityResources)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    foreach (var resource in Config.ApiScopes)
                    {
                        context.ApiScopes.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}

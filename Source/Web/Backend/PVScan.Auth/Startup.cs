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
using PVScan.Database.Extensions;

namespace PVScan.Auth
{
    public class Startup
    {
        IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // WOW WTF THE ORDER MATTERS OK
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration["DBConnection"];
            services.AddPVScanDatabase(connectionString);
            services.ConfigureIdentityServer(connectionString);

            services.AddControllersWithViews();

            services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "PVScan.Auth.Cookie";
                config.LoginPath = "/Auth/Login";
                config.LogoutPath = "/Auth/Logout";
                config.Cookie.SameSite = SameSiteMode.Lax;
            });

            services.AddAuthentication();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.EnsurePVScanDatabaseTablesCreated();
            app.EnsureIdentityServerDatabaseTablesCreated();

            // Todo: remove this and place info from config to secrets
            InitializeDatabase(app);

            app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var configurationContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                if (!configurationContext.Clients.Any())
                {
                    foreach (var client in Config.Clients)
                    {
                        configurationContext.Clients.Add(client.ToEntity());
                    }
                    configurationContext.SaveChanges();
                }

                if (!configurationContext.IdentityResources.Any())
                {
                    foreach (var resource in Config.IdentityResources)
                    {
                        configurationContext.IdentityResources.Add(resource.ToEntity());
                    }
                    configurationContext.SaveChanges();
                }

                if (!configurationContext.ApiScopes.Any())
                {
                    foreach (var resource in Config.ApiScopes)
                    {
                        configurationContext.ApiScopes.Add(resource.ToEntity());
                    }
                    configurationContext.SaveChanges();
                }
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PVScan.Database.Extensions;
using PVScan.API.Services.Interfaces;
using PVScan.API.Services;
using PVScan.API.Hubs;

namespace PVScan.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();

            services.AddControllers();

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "https://localhost:1488/";

                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        // Because I use ApiScopes and not ApiResources
                        ValidateAudience = false,
                        ValidateIssuer = false,
                    };
                    
                    // Todo: production code must use HTTPS
                    options.RequireHttpsMetadata = false;
                });

            services.AddAuthentication();
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim("scope", "PVScan.API").Build();
            });

            // Todo: narrow down to clients
            services.AddCors(confg =>
                confg.AddPolicy("AllowAll",
                    p => p.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()));

            var connectionString = Configuration["MySQLConnection"];

            services.AddPVScanDatabase(connectionString);

            services.AddTransient<IExperienceCalculator, ExperienceCalculator>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("AllowAll");

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization();
                endpoints.MapHub<BarcodesHub>("/hubs/barcodes").RequireAuthorization();
                endpoints.MapHub<UserInfoHub>("/hubs/userInfo").RequireAuthorization();
            });
        }
    }
}

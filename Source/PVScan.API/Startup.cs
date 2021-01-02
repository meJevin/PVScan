using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PVScan.Domain.Services;
using PVScan.EntityFramework;
using PVScan.EntityFramework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PVScan.API
{
    public class Startup
    {
        readonly IConfiguration Configuration;

        public Startup(IConfiguration cfg)
        {
            Configuration = cfg;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<PVScanDbContext>(cfg =>
            {
                cfg.UseSqlServer(Configuration["ConnectionStrings:Local"], o =>
                {
                    o.MigrationsAssembly(typeof(PVScanDbContext).Assembly.FullName);
                });
            });

            services.AddControllers();

            services.AddTransient<IBarcodeService, EFBarcodeService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

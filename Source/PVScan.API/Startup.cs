using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PVScan.EntityFramework;
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
            });
        }
    }
}

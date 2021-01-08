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
using PVScan.API.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            services.AddControllers();
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PVScan.API", Version = "v1" });
            //
            //    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            //    {
            //        Type = SecuritySchemeType.OAuth2,
            //        Flows = new OpenApiOAuthFlows
            //        {
            //            ClientCredentials = new OpenApiOAuthFlow
            //            {
            //                AuthorizationUrl = new Uri("https://localhost:44398/connect/authorize"),
            //                TokenUrl = new Uri("https://localhost:44398/connect/token"),
            //                Scopes = new Dictionary<string, string>
            //                {
            //                    {"PVScan.API", "Main PVScan API"}
            //                },
            //            },
            //        }
            //    });
            //
            //    c.OperationFilter<AuthorizeCheckOperationFilter>();
            //});

            // accepts any access token issued by identity server
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "https://localhost:44398/";

                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                    };
                });

            // adds an authorization policy to make sure the token is for scope 'api1'
            //services.AddAuthentication();
            //
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("Default", policy =>
            //    {
            //        policy.RequireAuthenticatedUser();
            //        policy.RequireClaim("scope", "PVScan.API");
            //    });
            //});
            services.AddCors(confg =>
                confg.AddPolicy("AllowAll",
                    p => p.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseDeveloperExceptionPage();
            //app.UseSwagger();
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PVScan.API v1");
            //
            //    c.OAuthClientId("client");
            //    c.OAuthAppName("Demo API - Swagger");
            //    c.OAuthUsePkce();
            //});
            app.UseCors("AllowAll");

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

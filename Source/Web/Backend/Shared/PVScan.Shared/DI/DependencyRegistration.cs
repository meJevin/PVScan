using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PVScan.API.Contract.Shared.Models;
using PVScan.Shared.Configurations;
using PVScan.Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PVScan.Shared.DI;
using PVScan.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PVScan.Shared.Services;
using PVScan.Shared.Middleware;

namespace PVScan.Shared.DI
{
    public static class DependencyRegistration
    {
        public static IServiceCollection AddRepositoriesFrom(this IServiceCollection services, Assembly assembly)
        {
            var implementedRepositories = assembly.GetImplementedRepositories();

            foreach (var tuple in implementedRepositories)
            {
                services.AddScoped(tuple.InterfaceType, tuple.ImplementationType);
            }

            return services;
        }

        public static IServiceCollection AddSharedSettings(this IServiceCollection services, IConfiguration configuration)
        {
            // Postgres settings
            services.Configure<PostgresSettings>(configuration.GetSection(nameof(PostgresSettings)));

            // Shared identity settings
            services.Configure<SharedIdentitySettings>(configuration.GetSection(nameof(SharedIdentitySettings)));

            return services;
        }

        public static IServiceCollection AddSharedServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<PopulateBaseResponseFilter>();

            return services;
        }

        public static IServiceCollection AddIdentityBase(this IServiceCollection services, IConfiguration configuration)
        {
            var sharedIdentitySettings = new SharedIdentitySettings();
            configuration.GetSection(nameof(SharedIdentitySettings)).Bind(sharedIdentitySettings);

            var publicKeyProvider = RsaExtensions.ParsePublicKey(sharedIdentitySettings.PublicKeyPath);
            var publicKey = new RsaSecurityKey(publicKeyProvider);

            services
                .AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.Audience = sharedIdentitySettings.Audience;
                    x.ClaimsIssuer = sharedIdentitySettings.Issuer;
                    x.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = sharedIdentitySettings.Issuer,
                        ValidAudience = sharedIdentitySettings.Audience,
                        IssuerSigningKey = publicKey,
                        ClockSkew = TimeSpan.Zero,
                    };

                    x.Events = new PVScanJwtBearerEvents();
                });

            return services;
        }

        public static IServiceCollection ConfigureApiBehaviorOptions(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors =
                        new ApiErrors
                        {
                            Errors = actionContext.ModelState
                                .Where(e => e.Value?.Errors?.Any() == true)
                                .Select(e =>
                                    new ApiError
                                    {
                                        Message = $"Couldn't parse the field value. Path: {e.Key}",
                                        Details = e.Value.Errors.First().ErrorMessage
                                    }).ToList()
                        };

                    return new BadRequestObjectResult(errors);
                };
            });

            return services;
        }

        public static void AddBaseResponsePopulationFilter(this MvcOptions options)
        {
            options.Filters.AddService<PopulateBaseResponseFilter>();
        }

        private static IList<(Type ImplementationType, Type InterfaceType)> GetImplementedRepositories(this Assembly assembly)
        {
            var repositories = assembly
               .GetTypes()
               .Select(t => IsImplementedRepository(t))
               .Where(a => a.ImplementationType != null && a.InterfaceType != null)
               .Select(a => (a.ImplementationType!, a.InterfaceType!))
               .ToList();

            return repositories;
        }

        private static (Type? ImplementationType, Type? InterfaceType) IsImplementedRepository(Type type)
        {
            var implementsIRepositoryInterface = type
                .GetInterfaces()
                .FirstOrDefault(a => 
                    a.IsGenericType && 
                    a.GetGenericTypeDefinition() == typeof(IRepository<>))
                is not null;

            if (!implementsIRepositoryInterface || type.IsAbstract)
            {
                return (null, null);
            }

            var baseClassInterfaces = type.BaseType is null ? Enumerable.Empty<Type>() : type.BaseType.GetInterfaces();
            var interfaceType = type
                .GetInterfaces()
                .Except(baseClassInterfaces)
                .FirstOrDefault();

            if (interfaceType is null)
            {
                return (null, null);
            }

            return (type, interfaceType);
        }
    }
}

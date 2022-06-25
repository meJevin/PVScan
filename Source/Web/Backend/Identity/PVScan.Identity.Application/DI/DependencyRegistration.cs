using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PVScan.Identity.Application.Services;
using PVScan.Identity.Application.Services.Interfaces;
using PVScan.Identity.Application.Validators;
using System.Reflection;

namespace PVScan.Identity.Application.DI
{
    public static class DependencyRegistration
    {
        public static IServiceCollection AddIdentityApplication(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = typeof(DependencyRegistration).Assembly;

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserSessionService, UserSessionService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();

            services.AddMediatR(assembly);
            services.AddValidatorsFromAssembly(assembly);

            return services;
        }
    }
}

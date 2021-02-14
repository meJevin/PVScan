using Autofac;
using PVScan.Mobile.DAL;
using PVScan.Mobile.Services;
using PVScan.Mobile.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PVScan.Mobile
{
    public static class Bootstrapper
    {
        public static void Initialize()
        {
            var containerBuilder = new ContainerBuilder();

            // DbContext from EF Core
            containerBuilder.RegisterType<PVScanMobileDbContext>()
                .AsSelf()
                .InstancePerLifetimeScope();

            // IdentityService singleton
            containerBuilder.RegisterType<IdentityService>()
                .As<IIdentityService>()
                .SingleInstance();

            containerBuilder.RegisterType<BarcodesRepository>()
                .As<IBarcodesRepository>()
                .InstancePerLifetimeScope();

            var container = containerBuilder.Build();
            Resolver.Initialize(container);
        }
    }
}

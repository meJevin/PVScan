using Autofac;
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

            // Register IdentityService singleton
            containerBuilder.RegisterType<IdentityService>()
                .As<IIdentityService>()
                .SingleInstance();

            var container = containerBuilder.Build();
            Resolver.Initialize(container);
        }
    }
}

using Autofac;
using MvvmHelpers;
using PVScan.Mobile.DAL;
using PVScan.Mobile.Services;
using PVScan.Mobile.Services.Interfaces;
using PVScan.Mobile.Views;
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

            // Barcodes repository
            containerBuilder.RegisterType<BarcodesRepository>()
                .As<IBarcodesRepository>()
                .InstancePerLifetimeScope();

            // Main page
            containerBuilder.RegisterType<MainPage>();

            // All VMs
            containerBuilder.RegisterAssemblyTypes(typeof(App).Assembly).
                Where(t => t.IsSubclassOf(typeof(BaseViewModel)));

            // KVP
            containerBuilder.RegisterType<PreferencesPersistentKVP>()
                .As<IPersistentKVP>()
                .InstancePerLifetimeScope();

            var container = containerBuilder.Build();
            Resolver.Initialize(container);
        }
    }
}

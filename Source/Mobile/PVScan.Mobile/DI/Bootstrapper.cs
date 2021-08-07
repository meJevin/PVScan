using Autofac;
using Microsoft.EntityFrameworkCore;
using MvvmHelpers;
using PVScan.Core.DAL;
using PVScan.Mobile.Services;
using PVScan.Core.Services.Interfaces;
using PVScan.Mobile.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using PVScan.Core;
using PVScan.Core.Services;
using PVScan.Mobile.Services.Interfaces;
using System.Reflection;

namespace PVScan.Mobile
{
    public class Bootstrapper
    {
        protected ContainerBuilder ContainerBuilder
        {
            get;
            private set;
        }

        public Bootstrapper()
        {
            Initialize();
            FinishInitialization();
        }

        protected virtual void Initialize()
        {
            ContainerBuilder = new ContainerBuilder();

            ContainerBuilder.RegisterType<PVScanDbContextFactory>()
                .As<IPVScanDbContextFactory>()
                .SingleInstance();

            // IdentityService singleton
            ContainerBuilder.RegisterType<IdentityService>()
                .As<IIdentityService>()
                .SingleInstance();

            // Barcodes repository
            ContainerBuilder.RegisterType<BarcodesRepository>()
                .As<IBarcodesRepository>()
                .InstancePerLifetimeScope();

            // Main page
            ContainerBuilder.RegisterType<MainPage>();

            // All VMs
            ContainerBuilder.RegisterAssemblyTypes(typeof(App).Assembly).
                Where(t => t.IsSubclassOf(typeof(BaseViewModel)));

            // KVP
            ContainerBuilder.RegisterType<PreferencesPersistentKVP>()
                .As<IPersistentKVP>()
                .InstancePerLifetimeScope();

            // Filter Service
            ContainerBuilder.RegisterType<BarcodesFilter>()
                .As<IBarcodesFilter>()
                .InstancePerLifetimeScope();

            // Sorter Service
            ContainerBuilder.RegisterType<BarcodeSorter>()
                .As<IBarcodeSorter>()
                .InstancePerLifetimeScope();

            // Http factory
#if DEBUG
            ContainerBuilder.RegisterType<DebugCertHttpClientFactory>()
                .As<IHttpClientFactory>()
                .InstancePerLifetimeScope();
#else
            ContainerBuilder.RegisterType<HttpClientFactory>()
                .As<IHttpClientFactory>()
                .InstancePerLifetimeScope();
#endif

            // Popup message Service
            ContainerBuilder.RegisterType<PopupMessageService>()
                .As<IPopupMessageService>()
                .SingleInstance();

            // PVSCan API
            ContainerBuilder.RegisterType<PVScanAPI>()
                .As<IPVScanAPI>()
                .InstancePerLifetimeScope();

            //// API Barcodes HUB Xamarin facade for main thread calls
            //ContainerBuilder.Register(ctx =>
            //{
            //    var barcodeHub = new APIBarcodeHub(ctx.Resolve<IIdentityService>());
            //    return new XamarinAPIBarcodeHubFacade(barcodeHub);
            //})
            //    .As<IAPIBarcodeHub>()
            //    .SingleInstance();
            
            // API Barcodes HUB
            ContainerBuilder.RegisterType<APIBarcodeHub>()
                .As<IAPIBarcodeHub>()
                .SingleInstance();

            // API User Info HUB Xamarin facade
            ContainerBuilder.RegisterType<APIUserInfoHub>()
                .As<IAPIUserInfoHub>()
                .SingleInstance();

            ContainerBuilder.RegisterType<BarcodeSynchronizer>()
                .As<IBarcodeSynchronizer>()
                .SingleInstance();
        }

        private void FinishInitialization()
        {
            var container = ContainerBuilder.Build();
            Resolver.Initialize(container);
        }
    }
}

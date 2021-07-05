using Autofac;
using Microsoft.EntityFrameworkCore;
using PVScan.Core.DAL;
using PVScan.Core.Services.Interfaces;
using PVScan.Desktop.WPF.Services.Interfaces;
using PVScan.Desktop.WPF.Services;
using System;
using System.Collections.Generic;
using System.Text;
using PVScan.Core;
using PVScan.Core.Services;
using System.Reflection;
using PVScan.Desktop.WPF.ViewModels;
using PVScan.Desktop.WPF.Views;
using Microsoft.Extensions.Configuration;
using PVScan.Desktop.WPF.Views.Popups;
using System.Windows;
using PVScan.Desktop.WPF.Services.SQLiteEncrypted;

namespace PVScan.Desktop.WPF.DI
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

        private void Initialize()
        {
            ContainerBuilder = new ContainerBuilder();

            // DbContext from EF Core
            ContainerBuilder.Register(ctx =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<PVScanDbContext>();

                optionsBuilder.UseSqlite(
                    $"Filename={DataAccess.DatabasePath}",
                    sqliteOptions =>
                    {
                        var migrationsAssembly = typeof(PVScanDbContext).GetTypeInfo().Assembly.GetName().Name;
                    });

                return new PVScanDbContext(optionsBuilder.Options);
            })
                .AsSelf()
                .InstancePerLifetimeScope();

            ContainerBuilder.Register(ctx =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<SQLiteEncryptedDbContext>();

                optionsBuilder.UseSqlite(
                    SQLiteEncryptedDbContext.GetConnection(Constants.SQLiteEncryptedKVPDatabasePath),
                    sqliteOptions =>
                    {
                        var migrationsAssembly = typeof(SQLiteEncryptedDbContext).GetTypeInfo().Assembly.GetName().Name;
                    });

                return new SQLiteEncryptedDbContext(optionsBuilder.Options);
            })
                .AsSelf()
                .InstancePerLifetimeScope();

            // Updater
            ContainerBuilder.Register(updater =>
            {
                var options = new UpdaterOptions()
                {
                    GitHubRepoURL = Constants.GitHubRepoURL,
                };

                return new Updater(options);
            })
                .As<IUpdater>()
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
            ContainerBuilder.RegisterType<MainWindow>();

            // All VMs
            ContainerBuilder.RegisterAssemblyTypes(typeof(App).Assembly).
                Where(t => t.IsSubclassOf(typeof(BaseViewModel)));

            // KVP
            ContainerBuilder.RegisterType<SQLiteEncryptedPersistentKVP>()
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

            //// Popup message Service
            //ContainerBuilder.RegisterType<PopupMessageService>()
            //    .As<IPopupMessageService>()
            //    .SingleInstance();

            // PVSCan API
            ContainerBuilder.RegisterType<PVScanAPI>()
                .As<IPVScanAPI>()
                .InstancePerLifetimeScope();

            // API Barcodes HUB
            ContainerBuilder.RegisterType<APIBarcodeHub>()
                .As<IAPIBarcodeHub>()
                .SingleInstance();

            // API User Info HUB
            ContainerBuilder.RegisterType<APIUserInfoHub>()
                .As<IAPIUserInfoHub>()
                .SingleInstance();

            // Barcode Image Reader
            ContainerBuilder.RegisterType<BarcodeReaderImage>()
                .As<IBarcodeReaderImage>()
                .InstancePerLifetimeScope();

            // IConfiguration
            ContainerBuilder.Register<IConfiguration>(ctx =>
            {
                var builder = new ConfigurationBuilder();
                builder.AddUserSecrets<App>();
                var cfg = builder.Build();

                return cfg;
            });

            ContainerBuilder.RegisterType<NoLocationAvailablePopup>()
                .As<IPopup<NoLocationAvailablePopupResult>>()
                .InstancePerLifetimeScope();
        }

        private void FinishInitialization()
        {
            var container = ContainerBuilder.Build();
            Resolver.Initialize(container);
        }
    }
}

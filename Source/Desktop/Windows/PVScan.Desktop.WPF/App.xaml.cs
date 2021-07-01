﻿using Microsoft.Extensions.Configuration;
using PVScan.Core;
using PVScan.Desktop.WPF.DI;
using PVScan.Desktop.WPF.Services.Interfaces;
using PVScan.Desktop.WPF.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PVScan.Desktop.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            InitializeDatabasePath();
            InitializeDependencyInjection();
            InitializeMapsServiceToken();
            await InitializeUpdater();

            ShowMainWindow();
        }

        private void ShowMainWindow()
        {
            var mainWindow = Resolver.Resolve<MainWindow>();

            MainWindow = mainWindow;
            MainWindow.Show();
        }

        private void InitializeDatabasePath()
        {
            var localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            var dbDirectory = Path.Combine(localAppDataFolder, "PVScan");

            if (!Directory.Exists(dbDirectory))
            {
                Directory.CreateDirectory(dbDirectory);
            }

            DataAccess.Init(Path.Combine(dbDirectory, "PVScan.db3"));
        }

        private void InitializeDependencyInjection()
        {
            _ = new Bootstrapper();
        }

        private void InitializeMapsServiceToken()
        {
            var cfg = Resolver.Resolve<IConfiguration>();
            //Constants.MapBoxKey = cfg.GetSection("MapBoxKey").Value;
            Constants.MapBoxKey = "N/A";
        }

        private async Task InitializeUpdater()
        {
            var updater = Resolver.Resolve<IUpdater>();
            await updater.InitializeAsync();

            _ = Task.Run(async () =>
            {
                await updater.CheckAndInstallUpdates();
            });
        }
    }
}

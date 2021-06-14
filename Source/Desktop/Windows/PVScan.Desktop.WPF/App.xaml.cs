using Microsoft.Extensions.Configuration;
using PVScan.Core;
using PVScan.Desktop.WPF.DI;
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
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            InitializeDatabasePath();
            InitializeDependencyInjection();
            InitializeMapsServiceToken();

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
            MapBoxToken = cfg.GetSection("MapBoxKey").Value;
        }

        public static string MapBoxToken;
    }
}

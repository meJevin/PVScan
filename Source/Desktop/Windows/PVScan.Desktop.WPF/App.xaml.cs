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
            DataAccess.Init(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PVScan.db3")
                );

            _ = new Bootstrapper();

            var mainWindow = Resolver.Resolve<MainWindow>();

            MainWindow = mainWindow;
            MainWindow.Show();
        }
    }
}

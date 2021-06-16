using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Squirrel;

namespace PVScan.Desktop.WPF.Services.Interfaces
{
    public interface IUpdater
    {
        Task InitializeAsync();
        Task CheckAndInstallUpdates();
        Task RestartApp();
    }
}

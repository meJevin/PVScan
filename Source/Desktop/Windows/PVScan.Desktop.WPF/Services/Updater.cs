using PVScan.Desktop.WPF.Services.Interfaces;
using Squirrel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Desktop.WPF.Services
{
    public class UpdaterOptions
    {
        public string GitHubRepoURL { get; set; }
    }

    public class Updater : IUpdater
    {
        private IUpdateManager _updateManager;
        private UpdaterOptions _options;

        public Updater(UpdaterOptions options)
        {
            _options = options;
        }

        public async Task InitializeAsync()
        {
            try
            {
                _updateManager = await UpdateManager.GitHubUpdateManager(_options.GitHubRepoURL);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not initialize update manager!");
            }
        }
        
        public async Task CheckAndInstallUpdates()
        {
            if (_updateManager == null)
            {
                await InitializeAsync();
            }

            if (_updateManager == null)
            {
                return;
            }

            var result = await _updateManager.CheckForUpdate();

            if (result.ReleasesToApply.Count > 0)
            {
                var releaseEntry = await _updateManager.UpdateApp();
            }
        }

        public async Task RestartApp()
        {
            UpdateManager.RestartApp();
        }
    }
}

using MvvmHelpers;
using PVScan.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace PVScan.Mobile.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        readonly IBarcodeSynchronizer Synchronizer;
        readonly IIdentityService IdentityService;

        public MainPageViewModel(
            IBarcodeSynchronizer synchronizer,
            IIdentityService identityService)
        {
            Synchronizer = synchronizer;
            IdentityService = identityService;

            LoadedCommand = new Command(async () =>
            {
                if (IdentityService.AccessToken != null)
                {
                    await Synchronizer.Synchronize();
                }
            });
        }

        public ICommand LoadedCommand { get; set; }
    }
}

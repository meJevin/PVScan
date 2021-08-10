using MvvmHelpers;
using PVScan.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace PVScan.Mobile.ViewModels
{
    // This VM is used a little fucky. Because I don't want to propogate this Binding Context
    // I set it manually in MainPage.xaml.cs and I don't bindind to anything acutally
    // I just call ICommands from wherever I please
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

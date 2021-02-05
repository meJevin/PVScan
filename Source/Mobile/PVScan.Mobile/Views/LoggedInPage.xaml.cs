using PVScan.Mobile.ViewModels;
using PVScan.Mobile.ViewModels.Messages.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PVScan.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoggedInPage : ContentView
    {
        public LoggedInPage()
        {
            InitializeComponent();

            var vm = (BindingContext as LoggedInPageViewModel);

            vm.SuccessfulLogout += Vm_SuccessfulLogout;
            vm.FailedLogout += Vm_FailedLogout;
        }

        private void Vm_SuccessfulLogout(object sender, LogoutEventArgs e)
        {
        }

        private void Vm_FailedLogout(object sender, LogoutEventArgs e)
        {
            // Failed to logout
        }
    }
}
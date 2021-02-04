using PVScan.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PVScan.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ApplicationSettingsPage : ContentView
    {
        public event EventHandler BackClicked;

        ApplicationSettingsViewModel vm;

        public ApplicationSettingsPage()
        {
            InitializeComponent();

            vm = BindingContext as ApplicationSettingsViewModel;
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            BackClicked.Invoke(sender, e);
        }

        private void AllowCamera_Toggled(object sender, ToggledEventArgs e)
        {
            if (vm == null)
            {
                return;
            }

            vm.AllowCameraCommand.Execute(null);
        }

        private void DarkTheme_Toggled(object sender, ToggledEventArgs e)
        {
            if (vm == null)
            {
                return;
            }

            vm.SwitchThemeCommand.Execute(null);
        }
    }
}
using PVScan.Mobile.ViewModels;
using PVScan.Mobile.ViewModels.Messages.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        LoggedInPageViewModel VM;

        public LoggedInPage()
        {
            InitializeComponent();

            VM = (BindingContext as LoggedInPageViewModel);

            VM.SuccessfulLogout += Vm_SuccessfulLogout;
            VM.FailedLogout += Vm_FailedLogout;

            if (Device.RuntimePlatform == Device.Android)
            {
                ProfileRefreshView.RefreshColor = Color.Black;
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                ProfileRefreshView.SetAppThemeColor(RefreshView.RefreshColorProperty, Color.Black, Color.White);
            }

            VM.PropertyChanged += VM_PropertyChanged;
        }

        private async void VM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LoggedInPageViewModel.IsUpdatingUserInfo))
            {
                if (VM.IsUpdatingUserInfo)
                {
                    VKLinkEntry.Opacity = 0.5;
                    IGLinkEntry.Opacity = 0.5;
                    SaveButton.Opacity = 0.5;
                    VKLinkEntry.InputTransparent = true;
                    IGLinkEntry.InputTransparent = true;
                    SaveButton.InputTransparent = true;
                }
                else if (!VM.IsError)
                {
                    VKLinkEntry.Opacity = 1;
                    IGLinkEntry.Opacity = 1;
                    SaveButton.Opacity = 1;
                    VKLinkEntry.InputTransparent = false;
                    IGLinkEntry.InputTransparent = false;
                    SaveButton.InputTransparent = false;
                }
            }
            else if (e.PropertyName == nameof(LoggedInPageViewModel.IsLogginOut))
            {
                if (VM.IsLogginOut)
                {
                    VKLinkEntry.Opacity = 0.5;
                    IGLinkEntry.Opacity = 0.5;
                    SaveButton.Opacity = 0.5;
                    LogoutButton.Opacity = 0.5;
                    VKLinkEntry.InputTransparent = true;
                    IGLinkEntry.InputTransparent = true;
                    SaveButton.InputTransparent = true;
                    LogoutButton.InputTransparent = true;
                }
                else if (!VM.IsError)
                {
                    VKLinkEntry.Opacity = 1;
                    IGLinkEntry.Opacity = 1;
                    SaveButton.Opacity = 1;
                    LogoutButton.Opacity = 1;
                    VKLinkEntry.InputTransparent = false;
                    IGLinkEntry.InputTransparent = false;
                    SaveButton.InputTransparent = false;
                    LogoutButton.InputTransparent = false;
                }
            }
            else if (e.PropertyName == nameof(LoggedInPageViewModel.IsInitializing))
            {
                if (VM.IsInitializing)
                {
                    ProfileContainer.IsVisible = false;
                    _ = ProfileContainer.FadeTo(0, 250, Easing.CubicOut);
                    _ = InitializingSpinner.FadeTo(1, 250, Easing.CubicOut);
                    await InitializingSpinner.ScaleTo(1, 250, Easing.CubicOut);
                }
                else if (!VM.IsError)
                {
                    ProfileContainer.IsVisible = true;
                    _ = ProfileContainer.FadeTo(1, 250, Easing.CubicOut);
                    _ = InitializingSpinner.FadeTo(0, 250, Easing.CubicOut);
                    await InitializingSpinner.ScaleTo(0.75, 250, Easing.CubicOut);
                }
            }
            else if (e.PropertyName == nameof(LoggedInPageViewModel.IsError))
            {
                if (VM.IsError)
                {
                    ProfileContainer.IsVisible = false;
                    _ = ProfileContainer.FadeTo(0, 250, Easing.CubicOut);
                    _ = SomethingWentWrongContainer.FadeTo(1, 250, Easing.CubicOut);
                    await SomethingWentWrongContainer.ScaleTo(1, 250, Easing.CubicOut);
                }
                else
                {
                    _ = SomethingWentWrongContainer.FadeTo(0, 250, Easing.CubicOut);
                    await SomethingWentWrongContainer.ScaleTo(0.75, 250, Easing.CubicOut);
                }
            }
        }

        public async Task Initialize()
        {
            ProfileContainer.Opacity = 0;
            IsVisible = true;
            _ = this.FadeTo(1, 250, Easing.CubicOut);
            _ = InitializingSpinner.FadeTo(1, 250, Easing.CubicOut);
            await InitializingSpinner.ScaleTo(1, 250, Easing.CubicOut);
            await (BindingContext as LoggedInPageViewModel).Initialize();
        }

        public async Task Uninitialize()
        {
            _ = this.FadeTo(0, 250, Easing.CubicOut);
            this.IsVisible = false;
            ProfileContainer.Opacity = 0;
            MainScrollView.ScrollToAsync(0, 0, false);
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
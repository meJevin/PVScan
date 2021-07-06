using PVScan.Core;
using PVScan.Desktop.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PVScan.Desktop.WPF.Views
{
    /// <summary>
    /// Interaction logic for LoggedInPage.xaml
    /// </summary>
    public partial class LoggedInPage : ContentControl
    {
        LoggedInPageViewModel VM;

        public LoggedInPage()
        {
            InitializeComponent();

            // Todo: this can be refactored
            if (DataContext is LoggedInPageViewModel vm)
            {
                VM = vm;
                VM.PropertyChanged += VM_PropertyChanged;
            }

            DataContextChanged += (newDataContext, obj) =>
            {
                if (newDataContext is LoggedInPageViewModel vm)
                {
                    VM = vm;
                }
            };
        }

        private async void VM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LoggedInPageViewModel.IsUpdatingUserInfo))
            {
                if (VM.IsUpdatingUserInfo)
                {
                    VKLinkEntry.IsEnabled = false;
                    IGLinkEntry.IsEnabled = false;
                    SaveButton.IsEnabled = false;
                }
                else if (!VM.IsError)
                {
                    VKLinkEntry.IsEnabled = true;
                    IGLinkEntry.IsEnabled = true;
                    SaveButton.IsEnabled = true;
                }
            }
            else if (e.PropertyName == nameof(LoggedInPageViewModel.IsLogginOut))
            {
                if (VM.IsLogginOut)
                {
                    VKLinkEntry.IsEnabled = false;
                    IGLinkEntry.IsEnabled = false;
                    SaveButton.IsEnabled = false;
                    LogoutButton.IsEnabled = false;
                }
                else if (!VM.IsError)
                {
                    VKLinkEntry.IsEnabled = true;
                    IGLinkEntry.IsEnabled = true;
                    SaveButton.IsEnabled = true;
                    LogoutButton.IsEnabled = true;
                }
            }
            else if (e.PropertyName == nameof(LoggedInPageViewModel.IsInitializing))
            {
                if (VM.IsInitializing)
                {
                    InitializingSpinner.Visibility = Visibility.Visible;
                    _ = ProfileContainer.FadeTo(0, Animations.DefaultDuration);
                    await InitializingSpinner.FadeTo(1, Animations.DefaultDuration);
                    ProfileContainer.Visibility = Visibility.Hidden;
                }
                else if (!VM.IsError)
                {
                    ProfileContainer.Visibility = Visibility.Visible;
                    _ = ProfileContainer.FadeTo(1, Animations.DefaultDuration);
                    await InitializingSpinner.FadeTo(0, Animations.DefaultDuration);
                    InitializingSpinner.Visibility = Visibility.Hidden;
                }
            }
            else if (e.PropertyName == nameof(LoggedInPageViewModel.IsError))
            {
                if (VM.IsError)
                {
                    SomethingWentWrongContainer.Visibility = Visibility.Visible;
                    _ = ProfileContainer.FadeTo(0, Animations.DefaultDuration);
                    await SomethingWentWrongContainer.FadeTo(1, Animations.DefaultDuration);
                    ProfileContainer.Visibility = Visibility.Hidden;
                }
                else
                {
                    await SomethingWentWrongContainer.FadeTo(0, Animations.DefaultDuration);
                    SomethingWentWrongContainer.Visibility = Visibility.Hidden;
                }
            }
        }

        public async Task Initialize()
        {
            ProfileContainer.Opacity = 0;
            ProfileContainer.Visibility = Visibility.Hidden;
            _ = InitializingSpinner.FadeTo(1, Animations.DefaultDuration);
            await VM.Initialize();
        }

        public async Task Uninitialize()
        {
            ProfileContainer.Opacity = 0;
            ProfileContainer.Visibility = Visibility.Hidden;
            MainScrollView.ScrollToTop();
        }
    }
}

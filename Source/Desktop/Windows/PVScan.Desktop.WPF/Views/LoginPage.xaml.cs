using PVScan.Core;
using PVScan.Desktop.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
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
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : ContentControl
    {
        public event EventHandler SignUpClicked;

        LoginPageViewModel VM;

        public LoginPage()
        {
            InitializeComponent();

            LoadingSpinnerContainer.Opacity = 0;

            // Todo: this can be refactored
            if (DataContext is LoginPageViewModel vm)
            {
                VM = vm;
                VM.PropertyChanged += VM_PropertyChanged;
            }

            DataContextChanged += (newDataContext, obj) =>
            {
                if (newDataContext is LoginPageViewModel vm)
                {
                    VM = vm;
                }
            };
        }

        private void VM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VM.IsLoggingIn))
            {
                if (VM.IsLoggingIn)
                {
                    _ = LoadingSpinnerContainer.FadeTo(1, Animations.DefaultDuration);
                }
                else
                {
                    _ = LoadingSpinnerContainer.FadeTo(0, Animations.DefaultDuration);
                }
            }
            else if (e.PropertyName == nameof(VM.Password))
            {
                if (PasswordBox.Password != VM.Password)
                {
                    PasswordBox.Password = VM.Password;
                }
            }
        }

        private void SignUpButton_Clicked(object sender, RoutedEventArgs e)
        {
            SignUpClicked?.Invoke(this, new EventArgs());
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            VM.Password = PasswordBox.Password;
        }
    }
}

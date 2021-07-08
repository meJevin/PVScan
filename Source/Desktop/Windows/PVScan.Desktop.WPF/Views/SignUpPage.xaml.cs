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
    /// Interaction logic for SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : ContentControl
    {
        public event EventHandler BackClicked;
        SignUpPageViewModel VM;

        public SignUpPage()
        {
            InitializeComponent();

            LoadingSpinnerContainer.Opacity = 0;

            // Todo: this can be refactored
            if (DataContext is SignUpPageViewModel vm)
            {
                VM = vm;
                VM.PropertyChanged += VM_PropertyChanged;
            }

            DataContextChanged += (newDataContext, obj) =>
            {
                if (newDataContext is SignUpPageViewModel vm)
                {
                    VM = vm;
                }
            };
        }

        private void VM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VM.IsSigningUp))
            {
                if (VM.IsSigningUp)
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

        private void BackButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            BackClicked?.Invoke(this, new EventArgs());
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            VM.Password = PasswordBox.Password;
        }
    }
}

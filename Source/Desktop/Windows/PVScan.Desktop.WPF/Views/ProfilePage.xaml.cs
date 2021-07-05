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
    /// Interaction logic for ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : ContentControl
    {
        ProfilePageViewModel VM;

        public ProfilePage()
        {
            InitializeComponent();

            if (DataContext is ProfilePageViewModel vm)
            {
                VM = vm;
            }

            DataContextChanged += (newDataContext, _) =>
            {
                if (newDataContext is ProfilePageViewModel vm)
                {
                    VM = vm;
                }
            };
        }

        public async Task Initialize()
        {
            if (VM.IsLoggedIn)
            {
                // Show and init logged in page
            }
            else
            {
                // Show sign up page
                LoginPage.Visibility = Visibility.Visible;
            }
        }
    }
}

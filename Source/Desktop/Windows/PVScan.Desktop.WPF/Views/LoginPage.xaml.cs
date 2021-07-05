using System;
using System.Collections.Generic;
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

        public LoginPage()
        {
            InitializeComponent();
        }

        private void SignUpButton_Clicked(object sender, RoutedEventArgs e)
        {
            SignUpClicked?.Invoke(this, new EventArgs());
        }
    }
}

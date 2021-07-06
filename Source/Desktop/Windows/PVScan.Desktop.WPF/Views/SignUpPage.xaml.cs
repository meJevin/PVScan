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
    /// Interaction logic for SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : ContentControl
    {
        public event EventHandler BackClicked;

        public SignUpPage()
        {
            InitializeComponent();
        }

        private void BackButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            BackClicked?.Invoke(this, new EventArgs());
        }
    }
}

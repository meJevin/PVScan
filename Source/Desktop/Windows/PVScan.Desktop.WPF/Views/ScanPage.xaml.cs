using Emgu.CV;
using MaterialDesignThemes.Wpf;
using PVScan.Desktop.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PVScan.Desktop.WPF.Views
{
    /// <summary>
    /// Interaction logic for ScanPage.xaml
    /// </summary>
    public partial class ScanPage : ContentControl
    {
        ScanPageViewModel VM;

        public ScanPage()
        {
            InitializeComponent();

            VM = (DataContext as ScanPageViewModel);

            VM.GotBarcode += VM_GotBarcode;
            VM.Cleared += VM_Cleared;
        }

        private void VM_Cleared(object sender, EventArgs e)
        {
            HidebarcodeInfoCard();
        }

        private void VM_GotBarcode(object sender, EventArgs e)
        {
            ShowBarcodeInfoCard();
        }

        private void ShowBarcodeInfoCard()
        {
            BarcodeInfoCard.TranslateTo(0, 0, TimeSpan.FromMilliseconds(250));
        }

        private void HidebarcodeInfoCard()
        {
            BarcodeInfoCard.TranslateTo(-350, 0, TimeSpan.FromMilliseconds(250));
        }
    }
}

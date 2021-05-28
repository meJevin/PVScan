using Emgu.CV;
using MaterialDesignThemes.Wpf;
using PVScan.Core;
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

            HidebarcodeInfoCard(TimeSpan.Zero);

            VM = (DataContext as ScanPageViewModel);

            VM.GotBarcode += VM_GotBarcode;
            VM.Cleared += VM_Cleared;
        }

        private void VM_Cleared(object sender, EventArgs e)
        {
            HidebarcodeInfoCard(Animations.DefaultDuration);
        }

        private void VM_GotBarcode(object sender, EventArgs e)
        {
            ShowBarcodeInfoCard(Animations.DefaultDuration);
        }

        private void ShowBarcodeInfoCard(TimeSpan duration)
        {
            BarcodeInfoCard.TranslateTo(0, 0, duration);
            SaveButton.TranslateTo(0, 0, duration);
        }

        private void HidebarcodeInfoCard(TimeSpan duration)
        {
            BarcodeInfoCard.TranslateTo(
                -(BarcodeInfoCard.Width + 
                BarcodeInfoCard.Margin.Left + 
                BarcodeInfoCard.Margin.Right), 0, duration);

            SaveButton.TranslateTo(0, 
                SaveButton.Height + 
                SaveButton.Margin.Top + 
                SaveButton.Margin.Bottom, duration);
        }
    }
}

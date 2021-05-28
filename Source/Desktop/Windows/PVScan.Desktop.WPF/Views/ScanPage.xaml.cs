using Emgu.CV;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
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
        Storyboard showScannedBarcodeInfo;
        Storyboard hideScannedBarcodeInfo;

        public ScanPage()
        {
            InitializeComponent();

            var showAnimation = new DoubleAnimation()
            {
                From = -350,
                To = 0,
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut },
                AutoReverse = false,
                Duration = new Duration(TimeSpan.FromMilliseconds(250)),
            };
            showScannedBarcodeInfo = new Storyboard();
            showScannedBarcodeInfo.Children.Add(showAnimation);
            Storyboard.SetTarget(showScannedBarcodeInfo, BarcodeInfoCardTransform);
            Storyboard.SetTargetProperty(showScannedBarcodeInfo, new PropertyPath("X"));

            showScannedBarcodeInfo.Begin();
        }
    }
}

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

        DoubleAnimation showAnimation;
        DoubleAnimation hideAnimation;

        public ScanPage()
        {
            InitializeComponent();

            showAnimation = new DoubleAnimation()
            {
                From = -350,
                To = 0,
                Duration = new Duration(TimeSpan.FromMilliseconds(250)),
                EasingFunction = new CubicEase()
                {
                    EasingMode = EasingMode.EaseOut,
                }
            };

            hideAnimation = new DoubleAnimation()
            {
                From = 0,
                To = -350,
                Duration = new Duration(TimeSpan.FromMilliseconds(250)),
                EasingFunction = new CubicEase()
                {
                    EasingMode = EasingMode.EaseOut,
                }
            };

            showScannedBarcodeInfo = new Storyboard();
            showScannedBarcodeInfo.Children.Add(showAnimation);
            Storyboard.SetTarget(showScannedBarcodeInfo, BarcodeInfoCardTransform);
            Storyboard.SetTargetProperty(showScannedBarcodeInfo, new PropertyPath("X"));

            hideScannedBarcodeInfo = new Storyboard();
            hideScannedBarcodeInfo.Children.Add(hideAnimation);
            Storyboard.SetTarget(hideScannedBarcodeInfo, BarcodeInfoCardTransform);
            Storyboard.SetTargetProperty(hideScannedBarcodeInfo, new PropertyPath("X"));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BarcodeInfoCardTransform.BeginAnimation(TranslateTransform.XProperty, showAnimation);
            //showScannedBarcodeInfo.Begin();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            BarcodeInfoCardTransform.BeginAnimation(TranslateTransform.XProperty, hideAnimation);
            //hideScannedBarcodeInfo.Begin();
        }
    }
}

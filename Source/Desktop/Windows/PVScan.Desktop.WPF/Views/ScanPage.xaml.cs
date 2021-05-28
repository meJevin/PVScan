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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BarcodeInfoCard.TranslateTo(0, 0, TimeSpan.FromMilliseconds(250));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            BarcodeInfoCard.TranslateTo(-350, 0, TimeSpan.FromMilliseconds(250));
        }
    }
}

using PVScan.Core;
using PVScan.Desktop.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel VM;

        public MainWindow()
        {
            InitializeComponent();

            VM = DataContext as MainWindowViewModel;
            VM.MapScanPagesToggled += VM_MapScanPagesToggled;

            _ = ToggleToScanPage(TimeSpan.Zero);
        }

        private async void VM_MapScanPagesToggled(object sender, EventArgs e)
        {
            if (ToggleScanMapPagesButton.Content.ToString() == "Map")
            {
                await ToggleToMapPage(Animations.DefaultDuration);
            }
            else
            {
                await ToggleToScanPage(Animations.DefaultDuration);
            }
        }

        private async Task ToggleToMapPage(TimeSpan duration)
        {
            ToggleScanMapPagesButton.Content = "Scan";
            MapPageView.Visibility = Visibility.Visible;

            ScanPageView.IsHitTestVisible = false;
            _ = ScanPageView.FadeTo(0, duration);

            MapPageView.IsHitTestVisible = true;
            await MapPageView.FadeTo(1, duration);

            ScanPageView.Visibility = Visibility.Hidden;
        }

        private async Task ToggleToScanPage(TimeSpan duration)
        {
            ToggleScanMapPagesButton.Content = "Map";
            ScanPageView.Visibility = Visibility.Visible;

            MapPageView.IsHitTestVisible = false;
            _ = MapPageView.FadeTo(0, duration);

            ScanPageView.IsHitTestVisible = true;
            await ScanPageView.FadeTo(1, duration);

            MapPageView.Visibility = Visibility.Hidden;
        }
    }
}

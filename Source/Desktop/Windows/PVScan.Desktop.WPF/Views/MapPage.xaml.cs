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
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;

namespace PVScan.Desktop.WPF.Views
{
    /// <summary>
    /// Interaction logic for MapPage.xaml
    /// </summary>
    public partial class MapPage : ContentControl
    {
        public MapPage()
        {
            InitializeComponent();
            Map.AccessToken = App.MapBoxToken;
            Map.Ready += Map_Ready;
        }

        private void Map_Ready(object sender, EventArgs e)
        {
            Map.Invoke.SetStyle("mapbox://styles/mapbox/dark-v10");
        }
    }
}

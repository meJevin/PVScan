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
            // InitializeMapControl();
        }

        private void InitializeMapControl()
        {
            Microsoft.Toolkit.Wpf.UI.Controls.MapControl map = new Microsoft.Toolkit.Wpf.UI.Controls.MapControl();

            map.MapServiceToken = App.MapServiceToken;
            MainContainer.Children.Add(map);

            var MyLandmarks = new List<MapElement>();

            BasicGeoposition snPosition = new BasicGeoposition { Latitude = 47.620, Longitude = -122.349 };
            Geopoint snPoint = new Geopoint(snPosition);

            var spaceNeedleIcon = new MapIcon
            {
                Location = snPoint,
                NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1.0),
                ZIndex = 0,
                Title = "Space Needle"
            };

            MyLandmarks.Add(spaceNeedleIcon);

            var LandmarksLayer = new MapElementsLayer
            {
                ZIndex = 1,
                MapElements = MyLandmarks
            };

            map.Layers.Add(LandmarksLayer);
            map.Center = snPoint;
            map.ZoomLevel = 14;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PVScan.Mobile.Views.DataTemplates
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NormalBarcodeItem : Grid
    {
        public event EventHandler Tapped;

        public NormalBarcodeItem()
        {
            InitializeComponent();
        }

        private void Barcode_Tapped(object sender, EventArgs e)
        {
            Tapped?.Invoke(sender, e);
        }
    }
}

using PVScan.Mobile.Models;
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
    public partial class NormalBarcodeItemDataTemplate : DataTemplate
    {
        public event EventHandler Tapped;
        public event EventHandler NoLocationTapped;

        public NormalBarcodeItemDataTemplate(Barcode b) 
            : base(() => CreateDataTemplate(b))
        {
            InitializeComponent();
        }

        static NormalBarcodeItem CreateDataTemplate(Barcode b)
        {
            var ret = new NormalBarcodeItem();
            ret.BindingContext = b;

            return ret;
        }

        private void Barcode_Tapped(object sender, EventArgs e)
        {
            Tapped?.Invoke(sender, e);
        }

        private void Barcode_NoLocationTapped(object sender, EventArgs e)
        {
            NoLocationTapped?.Invoke(sender, e);
        }

        //public NormalBarcodeItemDataTemplate()
        //{
        //    InitializeComponent();
        //}
    }
}
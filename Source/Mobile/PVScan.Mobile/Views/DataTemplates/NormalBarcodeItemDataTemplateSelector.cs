using PVScan.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace PVScan.Mobile.Views.DataTemplates
{
    public class NormalBarcodeItemDataTemplateSelector : DataTemplateSelector
    {
        public event EventHandler Tapped;
        public event EventHandler NoLocationTapped;

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var ret = new NormalBarcodeItemDataTemplate((Barcode)item);

            ret.Tapped += Tapped;
            ret.NoLocationTapped += NoLocationTapped;

            return ret;
        }
    }
}
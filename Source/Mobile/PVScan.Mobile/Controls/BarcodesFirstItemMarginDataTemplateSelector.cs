using PVScan.Mobile.Models;
using PVScan.Mobile.ViewModels;
using PVScan.Mobile.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PVScan.Mobile.Controls
{
    public class BarcodesFirstItemMarginDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NormarTemplate { get; set; }
        public DataTemplate MarginTemplate { get; set; }

        public HistoryPage Page { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is Barcode barcode)
            {
                var VM = Page.BindingContext as HistoryPageViewModel;

                if (VM.Barcodes.IndexOf(barcode) == 0)
                {
                    return MarginTemplate;
                }
            }

            return NormarTemplate;
        }
    }
}

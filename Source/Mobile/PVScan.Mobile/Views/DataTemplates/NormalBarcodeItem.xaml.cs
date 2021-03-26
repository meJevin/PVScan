using PVScan.Mobile.Models;
using PVScan.Mobile.Views.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PVScan.Mobile.Views.DataTemplates
{
    // Todo:
    /// IMPORTANT!!!!
    /// I remove effects because otherwise multiple selection doesn't work on iOS
    /// On android even with effects the selection works
    /// Need help! 
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NormalBarcodeItem : Grid
    {
        public event EventHandler Tapped;

        public static readonly BindableProperty IsEditableProperty =
            BindableProperty.Create(nameof(IsEditable), typeof(bool), typeof(NormalBarcodeItem), false,
                propertyChanged: OnIsEditableChanged);

        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        public static readonly BindableProperty HighlightedBarcodeProperty =
            BindableProperty.Create(nameof(HighlightedBarcode), typeof(Barcode), typeof(NormalBarcodeItem), null,
                propertyChanged: OnHighlightedBarcodeChanged);

        public Barcode HighlightedBarcode
        {
            get { return (Barcode)GetValue(HighlightedBarcodeProperty); }
            set { SetValue(HighlightedBarcodeProperty, value); }
        }

        static async void OnIsEditableChanged(BindableObject bindable, object oldValue, object newValue)
        {
            NormalBarcodeItem i = (NormalBarcodeItem)bindable;

            if ((bool)newValue == false)
            {
                await i.MakeNotEditable();
            }
            else
            {
                await i.MakeEditable();
            }
        }

        static async void OnHighlightedBarcodeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue == null)
            {
                return;
            }

            NormalBarcodeItem i = (NormalBarcodeItem)bindable;

            if ((Barcode)newValue == i.BindingContext)
            {
                await i.Highlight();
            }
        }

        IGestureRecognizer InnerContainerTapGestureRecognizer;

        public NormalBarcodeItem()
        {
            InitializeComponent();

            InnerContainerTapGestureRecognizer = InnerContainer.GestureRecognizers[0];
        }

        private void Barcode_Tapped(object sender, EventArgs e)
        {
            Tapped?.Invoke(sender, e);

            _ = Tap();
        }

        public async Task MakeEditable()
        {
            InnerContainer.GestureRecognizers.Remove(InnerContainerTapGestureRecognizer);

            _ = ImageLeftContainer.TranslateTo(0, 0, 250, Easing.CubicOut);
            _ = ImageLeftContainer.FadeTo(1, 250, Easing.CubicOut);
            await InfoContainer.PaddingLeftTo(54, 250, Easing.CubicOut);
        }

        public async Task MakeNotEditable()
        {
            InnerContainer.GestureRecognizers.Add(InnerContainerTapGestureRecognizer);
            
            _ = ImageLeftContainer.TranslateTo(-44, 0, 250, Easing.CubicOut);
            _ = ImageLeftContainer.FadeTo(0, 250, Easing.CubicOut);
            await InfoContainer.PaddingLeftTo(10, 250, Easing.CubicOut);
        }

        public async Task Tap()
        {
            await HighlightContainer.FadeTo(0.35, 75, Easing.CubicOut);
            await HighlightContainer.FadeTo(0, 100, Easing.CubicIn);
        }

        public async Task Highlight()
        {
            await HighlightContainer.FadeTo(0.35, 250, Easing.CubicOut);

            await Task.Delay(4000);

            await HighlightContainer.FadeTo(0, 1000, Easing.Linear);
        }
    }
}

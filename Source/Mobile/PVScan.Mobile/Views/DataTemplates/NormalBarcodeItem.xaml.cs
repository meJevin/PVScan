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
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NormalBarcodeItem : Grid
    {
        public event EventHandler Tapped;

        public static readonly BindableProperty IsEditableProperty =
            BindableProperty.Create(nameof(IsEditable), typeof(bool), typeof(NormalBarcodeItem), false,
                propertyChanged: OnEventNameChanged);

        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        static async void OnEventNameChanged(BindableObject bindable, object oldValue, object newValue)
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

        IList<IGestureRecognizer> InnerContainerGestureRecognizers = new List<IGestureRecognizer>();

        public NormalBarcodeItem()
        {
            InitializeComponent();

            foreach (var e in InnerContainer.GestureRecognizers)
            {
                InnerContainerGestureRecognizers.Add(e);
            }
        }

        private void Barcode_Tapped(object sender, EventArgs e)
        {
            Tapped?.Invoke(sender, e);
        }

        public async Task MakeEditable()
        {
            InnerContainer.GestureRecognizers.Clear();

            await InfoContainer.PaddingLeftTo(54, 250, Easing.CubicInOut);
            _ = ImageLeftContainer.TranslateTo(0, 0, 250, Easing.CubicOut);
            _ = ImageLeftContainer.FadeTo(1, 250, Easing.CubicOut);
        }

        public async Task MakeNotEditable()
        {
            foreach (var e in InnerContainerGestureRecognizers)
            {
                InnerContainer.GestureRecognizers.Add(e);
            }

            _ = ImageLeftContainer.TranslateTo(-44, 0, 250, Easing.CubicOut);
            _ = ImageLeftContainer.FadeTo(0, 250, Easing.CubicOut);
            await InfoContainer.PaddingLeftTo(10, 250, Easing.CubicInOut);
        }
    }
}

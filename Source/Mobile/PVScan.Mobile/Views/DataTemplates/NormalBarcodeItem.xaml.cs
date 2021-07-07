using PVScan.Core.Models;
using PVScan.Mobile.ViewModels;
using PVScan.Mobile.ViewModels.Messages;
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
        // Occurs when user taps on the barcode item
        public event EventHandler Tapped;

        // Occurs when user taps on the 'No location' indicator 
        public event EventHandler NoLocationTapped;

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
            NormalBarcodeItem i = (NormalBarcodeItem)bindable;

            if (newValue == null)
            {
                i.HighlightContainer.Opacity = 0;
                return;
            }

            if ((Barcode)newValue == i.BindingContext)
            {
                _ = i.Highlight();
            }
            else
            {
                i.HighlightContainer.Opacity = 0;
            }
        }

        IGestureRecognizer InnerContainerTapGestureRecognizer;

        public NormalBarcodeItem()
        {
            InitializeComponent();

            InnerContainerTapGestureRecognizer = InnerContainer.GestureRecognizers[0];

            // Initialization, basically
            BindingContextChanged += (_, _) =>
            {
                if (BindingContext == null)
                {
                    return;
                }

                ToggleFavoriteOpacity();
            };

            MessagingCenter.Subscribe(this, nameof(BarcodeFavotireToggledMessage),
                async (HistoryPageViewModel vm, BarcodeFavotireToggledMessage args) =>
                {
                    if ((BindingContext as Barcode) == args.UpdatedBarcode)
                    {
                        ToggleFavoriteOpacity();
                    }
                });

            MessagingCenter.Subscribe(this, nameof(BarcodeLocationSpecifiedMessage),
                async (SpecifyLocationPageViewModel vm, BarcodeLocationSpecifiedMessage args) =>
                {
                    if ((BindingContext as Barcode) == args.UpdatedBarcode)
                    {
                        NoLocationButton.IsVisible = false;
                        NoLocationButton.InputTransparent = true;
                    }
                });
        }

        private void ToggleFavoriteOpacity()
        {
            if ((BindingContext as Barcode).Favorite)
            {
                FavoriteButton.Opacity = 1;
            }
            else
            {
                FavoriteButton.Opacity = 0.15;
            }
        }

        private void Barcode_Tapped(object sender, EventArgs e)
        {
            Tapped?.Invoke(sender, e);
        }

        private void NoLocationButton_Clicked(object sender, EventArgs e)
        {
            NoLocationTapped.Invoke(sender, e);
        }

        public async Task MakeEditable()
        {
            InnerContainer.GestureRecognizers.Remove(InnerContainerTapGestureRecognizer);
            //BarcodeTextLabelOverlay.IsVisible = true;

            _ = ImageLeftContainer.TranslateTo(0, 0, 250, Easing.CubicOut);
            _ = ImageLeftContainer.FadeTo(1, 250, Easing.CubicOut);
            await InfoContainer.PaddingLeftTo(54, 250, Easing.CubicOut);
        }

        public async Task MakeNotEditable()
        {
            InnerContainer.GestureRecognizers.Add(InnerContainerTapGestureRecognizer);
            //BarcodeTextLabelOverlay.IsVisible = false;

            _ = ImageLeftContainer.TranslateTo(-44, 0, 250, Easing.CubicOut);
            _ = ImageLeftContainer.FadeTo(0, 250, Easing.CubicOut);
            await InfoContainer.PaddingLeftTo(10, 250, Easing.CubicOut);
        }

        public async Task Highlight()
        {
            await HighlightContainer.FadeTo(0.25, 250, Easing.CubicOut);

            await Task.Delay(1350);

            await HighlightContainer.FadeTo(0, 500, Easing.Linear);
        }
    }
}

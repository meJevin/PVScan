using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PVScan.Mobile.Converters;
using PVScan.Core.Models;
using PVScan.Mobile.ViewModels;
using PVScan.Mobile.ViewModels.Messages;
using Xamarin.Forms;
using Xamarin.Forms.Svg;

namespace PVScan.Mobile.Views
{
    public partial class BarcodeInfoPage : ContentView
    {
        public static readonly BindableProperty SelectedBarcodeProperty =
            BindableProperty.Create(nameof(SelectedBarcode), typeof(Barcode), typeof(BarcodeInfoPage), null,
                propertyChanged: OnSelectedBarcodeChanged);

        public static readonly BindableProperty ShowInListButtonVisibleProperty =
            BindableProperty.Create(nameof(SelectedBarcode), typeof(bool), typeof(BarcodeInfoPage), false,
                propertyChanged: OnShowInListButtonVisibleChanged);

        public static readonly BindableProperty ShowOnMapButtonVisibleProperty =
            BindableProperty.Create(nameof(SelectedBarcode), typeof(bool), typeof(BarcodeInfoPage), false,
                propertyChanged: OnShowOnMapButtonVisibleChanged);

        public Barcode SelectedBarcode
        {
            get { return (Barcode)GetValue(SelectedBarcodeProperty); }
            set { SetValue(SelectedBarcodeProperty, value); }
        }

        public bool ShowInListButtonVisible
        {
            get { return (bool)GetValue(ShowInListButtonVisibleProperty); }
            set { SetValue(ShowInListButtonVisibleProperty, value); }
        }

        public bool ShowOnMapButtonVisible
        {
            get { return (bool)GetValue(ShowOnMapButtonVisibleProperty); }
            set { SetValue(ShowOnMapButtonVisibleProperty, value); }
        }
        
        static async void OnSelectedBarcodeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var VM = (bindable.BindingContext as BarcodeInfoPageViewModel);
            var newBarcode = newValue as Barcode;

            VM.SelectedBarcode = newValue as Barcode;

            var page = (bindable as BarcodeInfoPage);

            // This can be done through bindings but this is a little cleaner in my opinion
            BarcodeImageConverter cnv = new BarcodeImageConverter();

            page.BarcodeImage.Source = (SvgImageSource)cnv.Convert(newBarcode, null, null, null);
            page.ToggleLocationLabel();
        }
        
        static async void OnShowInListButtonVisibleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as BarcodeInfoPage).ShowInListButton.IsVisible = (bool)newValue;
        }

        static async void OnShowOnMapButtonVisibleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as BarcodeInfoPage).ShowOnMapButton.IsVisible = (bool)newValue;
        }

        public event EventHandler<Barcode> Deleted;
        public event EventHandler<Barcode> ShowOnMap;
        public event EventHandler<Barcode> ShowInList;

        public event EventHandler<Barcode> NoLocationClicked;

        public BarcodeInfoPage()
        {
            InitializeComponent();

            ShowOnMapButton.IsVisible = ShowOnMapButtonVisible;
            ShowInListButton.IsVisible = ShowInListButtonVisible;

            ToggleLocationLabel();

            MessagingCenter.Subscribe(this, nameof(BarcodeLocationSpecifiedMessage),
                async (SpecifyLocationPageViewModel vm, BarcodeLocationSpecifiedMessage args) =>
                {
                    if (SelectedBarcode == args.UpdatedBarcode)
                    {
                        ToggleLocationLabel();

                        // :((((((( This is horrible but there is no way to update the ui otherwise 
                        BarcodeLocationLabel.RemoveBinding(Label.TextProperty);
                        BarcodeLocationLabel.SetBinding(Label.TextProperty, new Binding()
                        {
                            Path = "SelectedBarcode.ScanLocation",
                            Converter = new CoordinateStringConverter(),
                        });
                    }
                });
        }

        // Shows correct location label respecting the current selected barcode location
        public void ToggleLocationLabel()
        {
            if (SelectedBarcode == null || SelectedBarcode.ScanLocation == null)
            {
                ScanLocationAvailableContainer.IsVisible = false;
                ScanLocationNotAvailableContainer.IsVisible = true;
            }
            else
            {
                ScanLocationAvailableContainer.IsVisible = true;
                ScanLocationNotAvailableContainer.IsVisible = false;
            }
        }

        private void ShowOnMap_Clicked(object sender, EventArgs e)
        {
            ShowOnMap?.Invoke(this, SelectedBarcode);
        }

        private void Delete_Clicked(object sender, EventArgs e)
        {
            Deleted?.Invoke(this, SelectedBarcode);
        }

        public async Task ScrollToTop()
        {
            await ContentScrollView.ScrollToAsync(0, 0, false);
        }

        private void ShowInList_Clicked(object sender, EventArgs e)
        {
            ShowInList?.Invoke(this, SelectedBarcode);
        }

        private void NoLocationLabel_Tapped(object sender, EventArgs e)
        {
            NoLocationClicked?.Invoke(this, SelectedBarcode);
        }
    }
}

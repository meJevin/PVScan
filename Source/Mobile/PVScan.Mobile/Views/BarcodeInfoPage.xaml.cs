using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PVScan.Mobile.Models;
using PVScan.Mobile.ViewModels;
using Xamarin.Forms;

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
            (bindable.BindingContext as BarcodeInfoPageViewModel).SelectedBarcode = newValue as Barcode;
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

        public BarcodeInfoPage()
        {
            InitializeComponent();

            ShowOnMapButton.IsVisible = ShowOnMapButtonVisible;
            ShowInListButton.IsVisible = ShowInListButtonVisible;
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
    }
}

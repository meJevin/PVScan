using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using MvvmHelpers;
using Xamarin.Forms;

namespace PVScan.Mobile.Views.DataTemplates
{
    public partial class FilterBarcodeTypeItem : Grid
    {
        public static readonly BindableProperty SelectedBarcodeFormatsProperty =
            BindableProperty.Create(
                nameof(SelectedBarcodeFormats),
                typeof(ObservableRangeCollection<object>),
                typeof(FilterBarcodeTypeItem),
                new ObservableRangeCollection<object>(),
                propertyChanged: OnSelectedBarcodeFormatsChanged);

        public ObservableRangeCollection<object> SelectedBarcodeFormats
        {
            get { return (ObservableRangeCollection<object>)GetValue(SelectedBarcodeFormatsProperty); }
            set { SetValue(SelectedBarcodeFormatsProperty, value); }
        }

        public FilterBarcodeTypeItem()
        {
            InitializeComponent();

            SelectedBarcodeFormats.CollectionChanged += SelectedBarcodeFormats_CollectionChanged;
        }

        static async void OnSelectedBarcodeFormatsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            FilterBarcodeTypeItem i = (FilterBarcodeTypeItem)bindable;

            if (newValue != null)
            {
                (newValue as ObservableRangeCollection<object>).CollectionChanged += i.SelectedBarcodeFormats_CollectionChanged;
            }
        }

        private async void SelectedBarcodeFormats_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (SelectedBarcodeFormats.Contains(this.BindingContext))
            {
                await MakeSelected();
            }
            else
            {
                await MakeNotSelected();
            }
        }

        public async Task MakeSelected()
        {
            await CheckImage.FadeTo(1, 250, Easing.CubicOut);
        }

        public async Task MakeNotSelected()
        {
            await CheckImage.FadeTo(0, 250, Easing.CubicOut);
        }
    }
}

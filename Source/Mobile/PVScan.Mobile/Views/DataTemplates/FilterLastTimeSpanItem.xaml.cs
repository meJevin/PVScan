using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PVScan.Mobile.ViewModels;
using Xamarin.Forms;

namespace PVScan.Mobile.Views.DataTemplates
{
    public partial class FilterLastTimeSpanItem : Grid
    {
        public static readonly BindableProperty SelectedLastTimeSpanProperty =
            BindableProperty.Create(nameof(SelectedLastTimeSpan), typeof(LastTimeSpan), typeof(FilterLastTimeSpanItem), null,
                propertyChanged: OnSelectedLastTimeSpanChanged);

        public LastTimeSpan SelectedLastTimeSpan
        {
            get { return (LastTimeSpan)GetValue(SelectedLastTimeSpanProperty); }
            set { SetValue(SelectedLastTimeSpanProperty, value); }
        }

        static async void OnSelectedLastTimeSpanChanged(BindableObject bindable, object oldValue, object newValue)
        {
            FilterLastTimeSpanItem i = (FilterLastTimeSpanItem)bindable;

            if ((LastTimeSpan)newValue == i.BindingContext)
            {
                await i.MakeSelected();
            }
            else
            {
                await i.MakeNotSelected();
            }
        }

        public FilterLastTimeSpanItem()
        {
            InitializeComponent();
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

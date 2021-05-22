using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PVScan.Core.Models;
using PVScan.Mobile.ViewModels;
using Xamarin.Forms;

namespace PVScan.Mobile.Views.DataTemplates
{
    public partial class SortingFieldItem : Grid
    {
        public static readonly BindableProperty SelectedSortingFieldProperty =
            BindableProperty.Create(nameof(SelectedSortingField), typeof(SortingField), typeof(SortingFieldItem), null,
                propertyChanged: OnCurrentSortingChanged);

        public SortingField SelectedSortingField
        {
            get { return (SortingField)GetValue(SelectedSortingFieldProperty); }
            set { SetValue(SelectedSortingFieldProperty, value); }
        }

        static async void OnCurrentSortingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            SortingFieldItem i = (SortingFieldItem)bindable;

            if ((SortingField)newValue == (SortingField)i.BindingContext)
            {
                await i.MakeSelected();
            }
            else
            {
                await i.MakeNotSelected();
            }
        }

        public SortingFieldItem()
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

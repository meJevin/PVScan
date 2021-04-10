using PVScan.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PVScan.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FilterPage : ContentView
    {
        FilterPageViewModel VM;

        public FilterPage()
        {
            InitializeComponent();

            LayoutChanged += (s, e) =>
            {
                _ = ShowLastDateType(0);
            };

            VM = (BindingContext as FilterPageViewModel);
        }

        private async Task ShowLastDateType(uint duration = 250)
        {
            VM.DateFilterTypeIndex = 0;

            _ = DateTypeLastContainer.TranslateTo(0, 0, duration, Easing.CubicOut);
            _ = DateTypeIndicator.TranslateTo(0, 0, duration, Easing.CubicOut);
            await DateTypeRangeContainer.TranslateTo(DateTypeRangeContainer.Width, 0, duration, Easing.CubicOut);

            LastViewButton.Opacity = 1;
            RangeViewButton.Opacity = 0.35;
        }

        private async Task ShowRangeDateType(uint duration = 250)
        {
            VM.DateFilterTypeIndex = 1;

            _ = DateTypeRangeContainer.TranslateTo(0, 0, duration, Easing.CubicOut);
            _ = DateTypeIndicator.TranslateTo(DateTypeIndicator.Width, 0, duration, Easing.CubicOut);
            await DateTypeLastContainer.TranslateTo(-DateTypeLastContainer.Width, 0, duration, Easing.CubicOut);

            RangeViewButton.Opacity = 1;
            LastViewButton.Opacity = 0.35;
        }

        private async void LastDateType_Clicked(object sender, EventArgs e)
        {
            await ShowLastDateType();
        }

        private async void RangeDateType_Clicked(object sender, EventArgs e)
        {
            await ShowRangeDateType();
        }

        private async void ResetDateFilter_Tapped(object sender, EventArgs e)
        {
            await ShowLastDateType();
        }
    }
}
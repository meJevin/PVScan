using PVScan.Mobile.ViewModels;
using PVScan.Mobile.Views.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Effects;
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

        IGestureRecognizer InnerContainerTapGestureRecognizer;

        public NormalBarcodeItem()
        {
            InitializeComponent();

            InnerContainerTapGestureRecognizer = InnerContainer.GestureRecognizers[0];
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (this.BindingContext == null)
            {
                return;
            }

            AddTouchEffects();
        }

        private void AddTouchEffects()
        {
            Binding longPressBinding = new Binding();
            longPressBinding.Source = new RelativeBindingSource(RelativeBindingSourceMode.FindAncestorBindingContext,
                typeof(HistoryPageViewModel), 1);
            longPressBinding.Path = nameof(HistoryPageViewModel.CopyBarcodeToClipboardCommand);

            Binding longPressCommandBinding = new Binding();
            longPressCommandBinding.Source = this.BindingContext;

            InnerContainer.SetValue(TouchEffect.PressedScaleProperty, 0.95);
            InnerContainer.SetValue(TouchEffect.AnimationEasingProperty, Easing.CubicOut);
            InnerContainer.SetValue(TouchEffect.AnimationDurationProperty, 650);
            InnerContainer.SetBinding(TouchEffect.LongPressCommandProperty, longPressBinding);
            InnerContainer.SetBinding(TouchEffect.LongPressCommandParameterProperty, longPressCommandBinding);
        }

        private void RemoveTouchEffects()
        {
            InnerContainer.Effects.Remove(InnerContainer.Effects[0]);
            InnerContainer.ClearValue(TouchEffect.PressedScaleProperty);
            InnerContainer.ClearValue(TouchEffect.AnimationEasingProperty);
            InnerContainer.ClearValue(TouchEffect.AnimationDurationProperty);
            InnerContainer.RemoveBinding(TouchEffect.LongPressCommandProperty);
            InnerContainer.ClearValue(TouchEffect.LongPressCommandProperty);
            InnerContainer.RemoveBinding(TouchEffect.LongPressCommandParameterProperty);
            InnerContainer.ClearValue(TouchEffect.LongPressCommandParameterProperty);
        }

        private void Barcode_Tapped(object sender, EventArgs e)
        {
            Tapped?.Invoke(sender, e);
        }

        public async Task MakeEditable()
        {
            InnerContainer.GestureRecognizers.Remove(InnerContainerTapGestureRecognizer);
            RemoveTouchEffects();

            _ = ImageLeftContainer.TranslateTo(0, 0, 250, Easing.CubicOut);
            _ = ImageLeftContainer.FadeTo(1, 250, Easing.CubicOut);
            await InfoContainer.PaddingLeftTo(54, 250, Easing.CubicOut);
        }

        public async Task MakeNotEditable()
        {
            InnerContainer.GestureRecognizers.Add(InnerContainerTapGestureRecognizer);
            AddTouchEffects();

            _ = ImageLeftContainer.TranslateTo(-44, 0, 250, Easing.CubicOut);
            _ = ImageLeftContainer.FadeTo(0, 250, Easing.CubicOut);
            await InfoContainer.PaddingLeftTo(10, 250, Easing.CubicOut);
        }
    }
}

using PVScan.Mobile.Effects;
using PVScan.Mobile.iOS.Effects;
using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("PVScan")]
[assembly: ExportEffect(typeof(TooltipEffectiOS), nameof(TooltipEffect))]
namespace PVScan.Mobile.iOS.Effects
{
    public class TooltipEffectiOS : PlatformEffect
    {

        EasyTipView.EasyTipView tooltip;
        UITapGestureRecognizer tapGestureRecognizer;

        bool isShowing = false;

        public TooltipEffectiOS()
        {
            tooltip = new EasyTipView.EasyTipView();
            tooltip.DidDismiss += OnDismiss;
        }

        void OnTap(object sender, EventArgs e)
        {
            var control = Control ?? Container;

            var text = TooltipEffect.GetText(Element);

            if (isShowing)
            {
                tooltip?.Dismiss();
                isShowing = false;
                return;
            }

            if (!string.IsNullOrEmpty(text))
            {
                tooltip.BubbleColor = TooltipEffect.GetBackgroundColor(Element).ToUIColor();
                tooltip.ForegroundColor = TooltipEffect.GetTextColor(Element).ToUIColor();
                tooltip.Text = new Foundation.NSString(text);
                UpdatePosition();

                var window = UIApplication.SharedApplication.KeyWindow;
                var vc = window.RootViewController;
                while (vc.PresentedViewController != null)
                {
                    vc = vc.PresentedViewController;
                }

                tooltip?.Show(control, vc.View, true);
                isShowing = true;
            }
        }

        void OnDismiss(object sender, EventArgs e)
        {
            // do something on dismiss
        }

        protected override void OnAttached()
        {
            var control = Control ?? Container;

            if (control is UIButton)
            {
                var btn = Control as UIButton;
                btn.TouchUpInside += OnTap;
            }
            else
            {
                tapGestureRecognizer = new UITapGestureRecognizer((UITapGestureRecognizer obj) =>
                {
                    OnTap(obj, EventArgs.Empty);
                });
                control.UserInteractionEnabled = true;
                control.AddGestureRecognizer(tapGestureRecognizer);
            }
        }

        protected override void OnDetached()
        {

            var control = Control ?? Container;

            if (control is UIButton)
            {
                var btn = Control as UIButton;
                btn.TouchUpInside -= OnTap;
            }
            else
            {
                if (tapGestureRecognizer != null)

                    control.RemoveGestureRecognizer(tapGestureRecognizer);

            }
            tooltip?.Dismiss();
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            if (args.PropertyName == TooltipEffect.BackgroundColorProperty.PropertyName)
            {
                tooltip.BubbleColor = TooltipEffect.GetBackgroundColor(Element).ToUIColor();
            }
            else if (args.PropertyName == TooltipEffect.TextColorProperty.PropertyName)
            {
                tooltip.ForegroundColor = TooltipEffect.GetTextColor(Element).ToUIColor();
            }
            else if (args.PropertyName == TooltipEffect.TextProperty.PropertyName)
            {
                tooltip.Text = new Foundation.NSString(TooltipEffect.GetText(Element));
            }
            else if (args.PropertyName == TooltipEffect.PositionProperty.PropertyName)
            {
                UpdatePosition();
            }
        }

        void UpdatePosition()
        {
            var position = TooltipEffect.GetPosition(Element);
            switch (position)
            {
                case TooltipPosition.Top:
                    tooltip.ArrowPosition = EasyTipView.ArrowPosition.Bottom;
                    break;
                case TooltipPosition.Left:
                    tooltip.ArrowPosition = EasyTipView.ArrowPosition.Right;
                    break;
                case TooltipPosition.Right:
                    tooltip.ArrowPosition = EasyTipView.ArrowPosition.Left;
                    break;
                default:
                    tooltip.ArrowPosition = EasyTipView.ArrowPosition.Top;
                    break;
            }
        }
    }
}

using PVScan.Mobile.Android.Effects;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("PVScan")]
[assembly: ExportEffect(typeof(NoEntryVerticalPaddingEffect), nameof(NoEntryVerticalPaddingEffect))]
namespace PVScan.Mobile.Android.Effects
{
    public class NoEntryVerticalPaddingEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            Control.SetPadding(Control.PaddingLeft, 0, Control.PaddingRight, 0);
        }

        protected override void OnDetached()
        {
            throw new NotImplementedException();
        }
    }
}
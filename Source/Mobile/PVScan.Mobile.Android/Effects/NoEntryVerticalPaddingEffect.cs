using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PVScan.Mobile.Droid.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("PVScan")]
[assembly: ExportEffect(typeof(NoEntryVerticalPaddingEffect), nameof(NoEntryVerticalPaddingEffect))]
namespace PVScan.Mobile.Droid.Effects
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
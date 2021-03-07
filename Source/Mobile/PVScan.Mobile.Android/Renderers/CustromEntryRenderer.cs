using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PVScan.Mobile.Android.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Entry), typeof(CustromEntryRenderer))]
namespace PVScan.Mobile.Android.Renderers
{
    public class CustromEntryRenderer : EntryRenderer
    {
        public CustromEntryRenderer(Context context) : base(context)
        {

        }

        void SetColor(global::Android.Graphics.Color color)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Control.BackgroundTintList = ColorStateList.ValueOf(color);
            }
            else
            {
                Control.Background.SetColorFilter(color, PorterDuff.Mode.SrcAtop);
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                SetColor(global::Android.Graphics.Color.Transparent);

                this.EditText.FocusChange += (sender, ee) => {
                    bool hasFocus = ee.HasFocus;
                    if (hasFocus)
                    {
                        SetColor(global::Android.Graphics.Color.Transparent);
                    }
                    else
                    {
                        SetColor(global::Android.Graphics.Color.Transparent);
                    }
                };
            }
        }
    }
}
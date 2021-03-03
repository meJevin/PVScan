using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PVScan.Mobile.Views.Extensions
{
    public static class AnimationExtensions
    {
        public static async Task<bool> HeightTo(this View view, double height, uint duration = 250, Easing easing = null)
        {
            var tcs = new TaskCompletionSource<bool>();

            var heightAnimation = new Animation(x => view.HeightRequest = x, view.Height, height);
            heightAnimation.Commit(view, "HeightAnimation", 10, duration, easing, (finalValue, finished) => { tcs.SetResult(finished); });

            return await tcs.Task;
        }

        public static async Task<bool> WidthTo(this View view, double width, uint duration = 250, Easing easing = null)
        {
            var tcs = new TaskCompletionSource<bool>();

            var heightAnimation = new Animation(x => view.WidthRequest = x, view.Height, width);
            heightAnimation.Commit(view, "WidthAnimation", 10, duration, easing, (finalValue, finished) => { tcs.SetResult(finished); });

            return await tcs.Task;
        }

        public static async Task<bool> PaddingBottomTo(this Layout layout, double bottom, uint duration = 250, Easing easing = null)
        {
            var tcs = new TaskCompletionSource<bool>();

            var paddingAnimation = new Animation(x =>
            {
                layout.Padding = new Thickness()
                {
                    Top = layout.Padding.Top,
                    Bottom = x,
                    Left = layout.Padding.Left,
                    Right = layout.Padding.Right,
                };
            },

            layout.Padding.Bottom, bottom);
            paddingAnimation.Commit(layout, "WidthAnimation", 10, duration, easing, (finalValue, finished) => { tcs.SetResult(finished); });

            return await tcs.Task;
        }
    }
}

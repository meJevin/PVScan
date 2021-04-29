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

        public static async Task<bool> PaddingTopTo(this Layout layout, double top, uint duration = 250, Easing easing = null)
        {
            var tcs = new TaskCompletionSource<bool>();

            var paddingAnimation = new Animation(x =>
            {
                layout.Padding = new Thickness()
                {
                    Top = x,
                    Bottom = layout.Padding.Bottom,
                    Left = layout.Padding.Left,
                    Right = layout.Padding.Right,
                };
            },

            layout.Padding.Top, top);
            paddingAnimation.Commit(layout, "PaddingTopAnimation", 10, duration, easing, (finalValue, finished) => { tcs.SetResult(finished); });

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
            paddingAnimation.Commit(layout, "PaddingBottomAnimation", 10, duration, easing, (finalValue, finished) => { tcs.SetResult(finished); });

            return await tcs.Task;
        }

        public static async Task<bool> PaddingLeftTo(this Layout layout, double left, uint duration = 250, Easing easing = null)
        {
            var tcs = new TaskCompletionSource<bool>();

            var paddingAnimation = new Animation(x =>
            {
                layout.Padding = new Thickness()
                {
                    Top = layout.Padding.Top,
                    Bottom = layout.Padding.Bottom,
                    Left = x,
                    Right = layout.Padding.Right,
                };
            },

            layout.Padding.Left, left);
            paddingAnimation.Commit(layout, "PaddingLeftAnimation", 10, duration, easing, (finalValue, finished) => { tcs.SetResult(finished); });

            return await tcs.Task;
        }

        public static async Task<bool> PaddingRightTo(this Layout layout, double right, uint duration = 250, Easing easing = null)
        {
            var tcs = new TaskCompletionSource<bool>();

            var paddingAnimation = new Animation(x =>
            {
                layout.Padding = new Thickness()
                {
                    Top = layout.Padding.Top,
                    Bottom = layout.Padding.Bottom,
                    Left = layout.Padding.Left,
                    Right = x,
                };
            },

            layout.Padding.Right, right);
            paddingAnimation.Commit(layout, "PaddingRightAnimation", 10, duration, easing, (finalValue, finished) => { tcs.SetResult(finished); });

            return await tcs.Task;
        }

        public static async Task<bool> MarginBottomTo(this Layout layout, double bottom, uint duration = 250, Easing easing = null)
        {
            var tcs = new TaskCompletionSource<bool>();

            var paddingAnimation = new Animation(x =>
            {
                layout.Margin = new Thickness()
                {
                    Top = layout.Margin.Top,
                    Bottom = x,
                    Left = layout.Margin.Left,
                    Right = layout.Margin.Right,
                };
            },

            layout.Margin.Bottom, bottom);
            paddingAnimation.Commit(layout, "PaddingRightAnimation", 10, duration, easing, (finalValue, finished) => { tcs.SetResult(finished); });

            return await tcs.Task;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Windows.Foundation;

namespace PVScan.Desktop.WPF
{
    public static class Extensions
    {
        public static BitmapImage ToBitmapImageSource(this System.Drawing.Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            image.Freeze();

            return image;
        }

        public static async Task FadeTo(this UIElement element, double opacity, TimeSpan duration)
        {
            DoubleAnimation opacityAnimation = new DoubleAnimation()
            {
                EasingFunction = new CubicEase()
                {
                    EasingMode = EasingMode.EaseOut
                },
                Duration = duration,
            };

            opacityAnimation.From = element.Opacity;
            opacityAnimation.To = opacity;

            element.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);

            await Task.Delay(duration);
        }

        public static async Task TranslateTo(this UIElement element, double? X, double? Y, TimeSpan duration)
        {
            DoubleAnimation xAnimation = new DoubleAnimation()
            {
                EasingFunction = new CubicEase()
                {
                    EasingMode = EasingMode.EaseOut
                },
                Duration = duration,
            };
            DoubleAnimation yAnimation = new DoubleAnimation()
            {
                EasingFunction = new CubicEase()
                {
                    EasingMode = EasingMode.EaseOut
                },
                Duration = duration,
            };

            TranslateTransform transform = null; 

            if (element.RenderTransform is TranslateTransform)
            {
                transform = element.RenderTransform as TranslateTransform;
            }
            else if (element.RenderTransform is TransformGroup grp)
            {
                transform = (element.RenderTransform as TransformGroup).Children
                    .FirstOrDefault(t => t is TranslateTransform) as TranslateTransform;

                if (transform == null)
                {
                    grp.Children.Add(new TranslateTransform());
                    transform = grp.Children.Last() as TranslateTransform;
                }
            }
            else if (element.RenderTransform is MatrixTransform)
            {
                var group = new TransformGroup();
                group.Children.Add(new TranslateTransform());
                element.RenderTransform = group;
                transform = group.Children.Last() as TranslateTransform;
            }

            xAnimation.From = transform.X;
            yAnimation.From = transform.Y;

            // This stops the current animation
            transform.BeginAnimation(TranslateTransform.XProperty, null);
            transform.BeginAnimation(TranslateTransform.YProperty, null);

            xAnimation.To = X.HasValue ? X.Value : transform.X;
            yAnimation.To = Y.HasValue ? Y.Value : transform.Y;

            // Run animation on X and Y proper
            transform.BeginAnimation(TranslateTransform.XProperty, xAnimation);
            transform.BeginAnimation(TranslateTransform.YProperty, yAnimation);

            await Task.Delay(duration);
        }

        public static Task<TResult> AsTask<TResult>(this IAsyncOperation<TResult> operation)
        {
            // Create task completion result
            var tcs = new TaskCompletionSource<TResult>();

            // When the operation is completed...
            operation.Completed += delegate
            {
                switch (operation.Status)
                {
                    // If successful...
                    case AsyncStatus.Completed:
                        // Set result
                        tcs.TrySetResult(operation.GetResults());
                        break;
                    // If exception...
                    case AsyncStatus.Error:
                        // Set exception
                        tcs.TrySetException(operation.ErrorCode);
                        break;
                    // If canceled...
                    case AsyncStatus.Canceled:
                        // Set task as canceled
                        tcs.SetCanceled();
                        break;
                }
            };

            // Return the task
            return tcs.Task;
        }
    }
}

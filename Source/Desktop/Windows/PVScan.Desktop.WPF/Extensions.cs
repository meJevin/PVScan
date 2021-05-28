using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

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

        public static void TranslateTo(this UIElement element, double? X, double? Y, TimeSpan duration)
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
            else if (element.RenderTransform is TransformGroup)
            {
                transform = (element.RenderTransform as TransformGroup).Children
                    .FirstOrDefault(t => t is TranslateTransform) as TranslateTransform;
            }

            if (transform == null)
            {
                Console.WriteLine("Could not translate element because it has no translate transform");
                return;
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
        }
    }
}

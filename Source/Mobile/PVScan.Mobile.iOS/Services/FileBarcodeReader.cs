using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CoreGraphics;
using PVScan.Mobile.Services.Interfaces;
using UIKit;
using ZXing;
using ZXing.Mobile;

namespace PVScan.Mobile.iOS.Services
{

    public class FileBarcodeReader : IFileBarcodeReader
    {
        IBarcodeReader barcodeReader;

        public FileBarcodeReader()
        {
            barcodeReader = new BarcodeReader();
        }

        public async Task<Result> DecodeAsync(string filePath)
        {
            UIImage image = UIImage.FromFile(filePath);

            if (image.Orientation == UIImageOrientation.Right)
            {
                image = RotateImage(image, 90);
            }
            else if (image.Orientation == UIImageOrientation.Left)
            {
                image = RotateImage(image, -90);
            }

            image = ResizeImage(image, 1000, 1000);

            image = CompressImage(image);

            var lSrc = new RGBLuminanceSourceiOS(image);

            var result = barcodeReader.Decode(lSrc);

            return result;
        }

        private UIImage RotateImage(UIImage image, float degrees)
        {
            float Radians = degrees * (float)Math.PI / 180;

            UIView view = new UIView(frame: new CGRect(0, 0, image.Size.Height, image.Size.Width));
            CGAffineTransform t = CGAffineTransform.MakeRotation(Radians);
            view.Transform = t;
            CGSize size = view.Frame.Size;

            UIGraphics.BeginImageContext(size);
            CGContext context = UIGraphics.GetCurrentContext();

            context.TranslateCTM(size.Height / 2, size.Width / 2);
            context.RotateCTM(Radians);
            context.ScaleCTM(1, -1);

            context.DrawImage(new CGRect(-image.Size.Width / 2, -image.Size.Height / 2, image.Size.Height, image.Size.Width), image.CGImage);

            UIImage imageCopy = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return imageCopy;
        }

        private UIImage CompressImage(UIImage image)
        {
            var jpegData = image.AsJPEG(0.85f);

            return UIImage.LoadFromData(jpegData, 1);
        }

        private UIImage ResizeImage(UIImage image, float maxWidth, float maxHeight)
        {
            var sourceSize = image.Size;
            var maxResizeFactor = Math.Min(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
            if (maxResizeFactor > 1) return image;
            var width = maxResizeFactor * sourceSize.Width;
            var height = maxResizeFactor * sourceSize.Height;
            UIGraphics.BeginImageContext(new CGSize(width, height));
            image.Draw(new CGRect(0, 0, width, height));
            var resultImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return resultImage;
        }
    }
}

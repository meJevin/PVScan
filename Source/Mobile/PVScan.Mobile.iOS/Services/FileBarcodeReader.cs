using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CoreGraphics;
using PVScan.Mobile.Services.Interfaces;
using UIKit;
using ZXing;

namespace PVScan.Mobile.iOS.Services
{
    public class FileBarcodeReader : IFileBarcodeReader
    {
        BarcodeReaderGeneric barcodeReader;

        public FileBarcodeReader()
        {
            barcodeReader = new BarcodeReader();
        }

        public async Task<Result> DecodeAsync(string filePath, Stream stream)
        {
            UIImage image = UIImage.FromFile(filePath);

            var width = (int)image.CGImage.Width;
            var height = (int)image.CGImage.Height;

            CGImage imageRef = image.CGImage;
            CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB();
            byte[] rawData = new byte[height * width * 4];
            int bytesPerPixel = 4;
            int bytesPerRow = bytesPerPixel * width;
            int bitsPerComponent = 8;
            CGContext context = new CGBitmapContext(rawData, width, height,
                            bitsPerComponent, bytesPerRow, colorSpace,
                            CGBitmapFlags.PremultipliedLast | CGBitmapFlags.ByteOrder32Big);
            context.DrawImage(new CGRect(0, 0, width, height), imageRef);

            LuminanceSource lSrc = new RGBALuminanceSource(
                rawData, rawData.Length, width, height);

            var result = barcodeReader.Decode(lSrc);

            return result;
        }
    }

    public class RGBALuminanceSource : BaseLuminanceSource
    {
        public RGBALuminanceSource(byte[] cvPixelByteArray, int cvPixelByteArrayLength, int width, int height)
            : base(width, height) => CalculateLuminance(cvPixelByteArray, cvPixelByteArrayLength);

        public RGBALuminanceSource(byte[] luminances, int width, int height) : base(luminances, width, height)
        {
        }

        void CalculateLuminance(byte[] rgbRawBytes, int bytesLen)
        {
            for (int rgbIndex = 0, luminanceIndex = 0; rgbIndex < bytesLen && luminanceIndex < luminances.Length; luminanceIndex++)
            {
                // Calculate luminance cheaply, favoring green.
                var r = rgbRawBytes[rgbIndex++];
                var g = rgbRawBytes[rgbIndex++];
                var b = rgbRawBytes[rgbIndex++];
                var alpha = rgbRawBytes[rgbIndex++];
                var luminance = (byte)((RChannelWeight * r + GChannelWeight * g + BChannelWeight * b) >> ChannelWeight);
                luminances[luminanceIndex] = (byte)(((luminance * alpha) >> 8) + (255 * (255 - alpha) >> 8));
            }
        }

        protected override LuminanceSource CreateLuminanceSource(byte[] newLuminances, int width, int height)
            => new RGBALuminanceSource(newLuminances, width, height);
    }
}

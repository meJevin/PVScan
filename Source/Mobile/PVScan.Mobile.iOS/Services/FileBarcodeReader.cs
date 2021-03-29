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
        BarcodeReaderGeneric barcodeReader;

        public FileBarcodeReader()
        {
            barcodeReader = new BarcodeReaderGeneric();
        }

        public async Task<Result> DecodeAsync(string filePath)
        {
            // Notice, that this image is rotated 90 degrees! But that's ok
            UIImage image = UIImage.FromFile(filePath);

            var lSrc = new RGBLuminanceSourceiOS(image);

            //CGImage i = LSrcToCGImage((int)image.CGImage.Width, (int)image.CGImage.Height, lSrc.Matrix);

            // Decode?
            var result = barcodeReader.Decode(lSrc);

            return result;
        }

        private CGImage LSrcToCGImage(int width, int height, byte[] lSrc)
        {
            var bytesPerPixel = 4;
            var bitsPerComponent = 8;
            var bytesPerUInt32 = sizeof(UInt32) / sizeof(byte);

            var bytesPerRow = bytesPerPixel * width;
            var numOfBytes = height * width * bytesPerUInt32;

            IntPtr pixelPtr = IntPtr.Zero;
            try
            {
                pixelPtr = Marshal.AllocHGlobal((int)numOfBytes);

                using (var colorSpace = CGColorSpace.CreateDeviceRGB())
                {
                    CGImage newCGImage;
                    using (var context = new CGBitmapContext(pixelPtr, width, height, bitsPerComponent, bytesPerRow, colorSpace, CGImageAlphaInfo.PremultipliedLast))
                    {
                        unsafe
                        {
                            var currentPixel = (byte*)pixelPtr.ToPointer();
                            for (int i = 0; i < height; i++)
                            {
                                for (int j = 0; j < width; j++)
                                {
                                    // RGBA8888 pixel format
                                    *currentPixel = lSrc[i * width + j];
                                    *(currentPixel + 1) = lSrc[i * width + j];
                                    *(currentPixel + 2) = lSrc[i * width + j];
                                    *(currentPixel + 3) = 255;
                                    currentPixel += 4;
                                }
                            }
                        }
                        newCGImage = context.ToImage();
                    }
                    return newCGImage;
                }
            }
            finally
            {
                if (pixelPtr != IntPtr.Zero)
                    Marshal.FreeHGlobal(pixelPtr);
            }
        }
    }
}

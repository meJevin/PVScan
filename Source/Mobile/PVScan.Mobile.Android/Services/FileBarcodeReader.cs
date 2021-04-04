using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Android.Graphics;
using PVScan.Mobile.Services.Interfaces;
using ZXing;

namespace PVScan.Mobile.Droid.Services
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
            var options = new BitmapFactory.Options
            {
                InJustDecodeBounds = true
            };

            var bitmap = BitmapFactory.DecodeFile(filePath, options);

            var width = options.OutWidth;
            var height = options.OutHeight;
            var size = width * height * 4;

            var pixelData = new byte[size];

            var byteBuffer = Java.Nio.ByteBuffer.AllocateDirect(size);
            bitmap.CopyPixelsToBuffer(byteBuffer);
            Marshal.Copy(byteBuffer.GetDirectBufferAddress(), pixelData, 0, size);
            byteBuffer.Dispose();

            var lSrc = new RGBLuminanceSource(pixelData, width, height);

            var result = barcodeReader.Decode(lSrc);

            return result;
        }
    }
}

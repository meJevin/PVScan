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

            var lSrc = new RGBLuminanceSourceiOS(image);

            var result = barcodeReader.Decode(lSrc);

            return result;
        }
    }
}

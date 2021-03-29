using System;
using System.IO;
using System.Threading.Tasks;
using PVScan.Mobile.Services.Interfaces;
using ZXing;

namespace PVScan.Mobile.Droid.Services
{
    public class FileBarcodeReader : IFileBarcodeReader
    {
        BarcodeReaderGeneric barcodeReader;

        public FileBarcodeReader()
        {
            barcodeReader = new BarcodeReaderGeneric();
        }

        public Task<Result> DecodeAsync(string filePath, Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}

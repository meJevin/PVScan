using System;
using System.IO;
using System.Threading.Tasks;
using ZXing;

namespace PVScan.Mobile.Services.Interfaces
{
    // Takes raw bytes of a picture (hopefully) and spits out a ZXING results
    public interface IFileBarcodeReader
    {
        Task<Result> DecodeAsync(string filePath);
    }
}

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using ZXing;

namespace PVScan.Core.Models
{
    public class Barcode
    {
        public int Id { get; set; }
        public BarcodeFormat Format { get; set; }
        public string Text { get; set; }
        public Coordinate ScanLocation { get; set; }
        public DateTime ScanTime { get; set; }
        public bool Favorite { get; set; }
        public string Hash { get; set; }
        public string GUID { get; set; }

        public static string HashOf(Barcode barcode)
        {
            string input = "";
            input += barcode.Format.ToString() + " ";
            input += barcode.Text.ToString() + " ";
            input += barcode.ScanLocation?.Latitude?.ToString() + " ";
            input += barcode.ScanLocation?.Longitude?.ToString() + " ";
            input += barcode.ScanTime.Ticks.ToString() + " ";
            input += barcode.Favorite.ToString() + " ";
            input += barcode.GUID?.ToString() + " ";

            using HashAlgorithm algorithm = SHA256.Create();
            var hashBytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}

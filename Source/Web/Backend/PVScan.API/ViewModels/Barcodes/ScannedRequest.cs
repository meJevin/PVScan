using PVScan.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PVScan.API.ViewModels.Barcodes
{
    public class ScannedRequest
    {
        public string Text { get; set; }
        public BarcodeFormat Format { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime ScanTime { get; set; }
        public bool Favorite { get; set; }
        public string Hash { get; set; }
        public string GUID { get; set; }
    }
}

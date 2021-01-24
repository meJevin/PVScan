using System;
using System.Collections.Generic;
using System.Text;
using ZXing;

namespace PVScan.Mobile.Models
{
    public class Barcode
    {
        public int Id { get; set; }
        public BarcodeFormat Format { get; set; }
        public string Text { get; set; }
        public Coordinate ScanLocation { get; set; }
        public bool ServerSynced { get; set; }
    }
}

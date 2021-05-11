using PVScan.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PVScan.Domain.Entities
{
    public class Barcode
    {
        public int Id { get; set; }
        public BarcodeFormat Format { get; set; }
        public string Text { get; set; }
        public Coordinate ScanLocation { get; set; }
        public string UserId { get; set; }
        public DateTime ScanTime { get; set; }
        public bool Favorite { get; set; }
        public string GUID { get; set; } 
    }
}

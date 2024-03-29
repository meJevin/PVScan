﻿using System;
using System.Collections.Generic;
using System.Text;
using ZXing;

namespace PVScan.Core.Models.API
{
    public class UpdatedBarcodeRequest
    {
        public string GUID { get; set; }
        public bool Favorite { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime LastTimeUpdated { get; set; }
    }
}

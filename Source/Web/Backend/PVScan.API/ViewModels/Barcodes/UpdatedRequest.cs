using PVScan.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PVScan.API.ViewModels.Barcodes
{
    // We can only update barcode location, favorite
    // I.e. it was unavailable and user specified it
    public class UpdatedRequest
    {
        public string GUID { get; set; }
        public bool Favorite { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}

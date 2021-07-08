using System;
using System.Collections.Generic;
using System.Text;

namespace PVScan.Core.Models.API
{
    public class SynchronizeRequest
    {
        // GUIDs and Hashes of local barcodes from device
        public IEnumerable<LocalBarcodeInfo> LocalBarcodeInfos { get; set; }
    }
}

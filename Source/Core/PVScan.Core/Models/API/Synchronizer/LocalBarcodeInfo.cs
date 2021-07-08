using System;
using System.Collections.Generic;
using System.Text;

namespace PVScan.Core.Models.API
{
    public class LocalBarcodeInfo
    {
        // Id of local device DB
        public int LocalId { get; set; }
        public string GUID { get; set; }
        public string Hash { get; set; }
        public DateTime LastTimeUpdated { get; set; }
    }

}

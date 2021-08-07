using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PVScan.Domain.DTO.Synchronizer
{
    public class LocalBarcodeInfo
    {
        // Id of local device DB
        public int LocalId { get; set; }
        public string GUID { get; set; }
        public string Hash { get; set; }
        public DateTime LastTimeUpdated { get; set; }
    }

    public class SynchronizeRequest
    {
        public IEnumerable<LocalBarcodeInfo> LocalBarcodeInfos { get; set; }
    }
}

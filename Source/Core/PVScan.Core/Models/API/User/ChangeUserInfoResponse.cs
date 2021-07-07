using System;
using System.Collections.Generic;
using System.Text;

namespace PVScan.Core.Models.API
{
    public class ChangeUserInfoResponse
    {
        public int BarcodesScanned { get; set; }
        public int BarcodeFormatsScanned { get; set; }
        public double Experience { get; set; }
        public int Level { get; set; }
        public string VKLink { get; set; }
        public string IGLink { get; set; }
    }
}

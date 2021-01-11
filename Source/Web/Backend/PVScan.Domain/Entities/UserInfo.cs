using System;
using System.Collections.Generic;
using System.Text;

namespace PVScan.Domain.Entities
{
    public class UserInfo
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int BarcodesScanned { get; set; }
        public int BarcodeFormatsScanned { get; set; }
        public double Experience { get; set; }
        public int Level { get; set; }
        public string VKLink { get; set; }
        public string IGLink { get; set; }
    }
}

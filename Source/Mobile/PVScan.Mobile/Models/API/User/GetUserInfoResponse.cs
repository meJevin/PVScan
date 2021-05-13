using System;
using System.Collections.Generic;
using System.Text;

namespace PVScan.Mobile.Models.API
{
    public class GetUserInfoResponse
    {
        public int BarcodesScanned { get; set; }
        public int BarcodeFormatsScanned { get; set; }
        public double Experience { get; set; }
        public int Level { get; set; }
        public string VKLink { get; set; }
        public string IGLink { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
    }
}

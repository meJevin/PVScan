using System;
using System.Collections.Generic;
using System.Text;

namespace PVScan.Core.Models.API
{
    public class ScannedBarcodeResponse
    {
        //public double ExperienceGained { get; set; }
        //public int LevelsGained { get; set; }
        public double UserExperience { get; set; }
        public int UserLevel { get; set; }
        public int UserBarcodesScanned { get; set; }
        public int UserBarcodeFormatsScanned { get; set; }
    }
}

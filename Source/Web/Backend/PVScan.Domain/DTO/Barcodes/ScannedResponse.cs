using PVScan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PVScan.Domain.DTO.Barcodes
{
    public class ScannedResponse
    {
        public double ExperienceGained { get; set; }
        public int LevelsGained { get; set; }
        public double UserExperience { get; set; }
        public int UserLevel { get; set; }
        public int UserBarcodesScanned { get; set; }
        public int UserBarcodeFormatsScanned { get; set; }
    }
}

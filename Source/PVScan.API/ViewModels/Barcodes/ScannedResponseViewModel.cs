using PVScan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PVScan.API.ViewModels.Barcodes
{
    public class ScannedResponseViewModel
    {
        public double ExperienceGained { get; set; }
        public Barcode Barcode { get; set; }
    }
}

using System;
using System.Collections.Generic;
using ZXing;

namespace PVScan.Mobile.Models
{
    // Enum for filter page to filter by last day/week/month/year
    public enum LastTimeType
    {
        Day,
        Week,
        Month,
        Year,
    }

    public class Filter
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public LastTimeType? LastType { get; set; }

        public IEnumerable<BarcodeFormat> BarcodeFormats { get; set; }
    }
}

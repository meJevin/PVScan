using System;
using System.Collections.Generic;
using ZXing;

namespace PVScan.Core.Models
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

        public IList<BarcodeFormat> BarcodeFormats { get; set; }

        public static Filter Default()
        {
            return new Filter()
            {
                BarcodeFormats = new List<BarcodeFormat>(),
                LastType = null,
                FromDate = null,
                ToDate = null,
            };
        }
    }
}

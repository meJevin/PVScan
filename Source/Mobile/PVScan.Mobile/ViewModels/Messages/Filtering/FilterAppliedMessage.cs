using System;
using System.Collections.Generic;
using System.Text;
using PVScan.Core.Models;
using ZXing;

namespace PVScan.Mobile.ViewModels.Messages.Filtering
{
    public class FilterAppliedMessage
    {
        public Filter NewFilter { get; set; }
    }
}

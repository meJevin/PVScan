using System;
using System.Collections.Generic;
using System.Text;
using PVScan.Core.Models;
using ZXing;
using PVScan.Mobile.ViewModels;

namespace PVScan.Mobile.ViewModels.Messages
{
    public class SortingAppliedMessage
    {
        public Sorting NewSorting { get; set; }
    }
}

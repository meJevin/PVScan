using System;
using System.Collections.Generic;
using System.Linq;
using ZXing.Mobile;

namespace PVScan.Mobile
{
    public static class PVScanExtension
    {
        public static CameraResolution Best(this List<CameraResolution> resolutions)
        {
            CameraResolution maxResolution = resolutions.First();

            foreach (var res in resolutions)
            {
                if (res.Width * res.Height > maxResolution.Height * maxResolution.Width)
                {
                    maxResolution = res;
                }
            }

            return maxResolution;
        }
    }
}

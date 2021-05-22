using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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

        public static string ToTitleCase(this string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
        }

        public static async Task<HttpResponseMessage> WithTimeout(
            this Task<HttpResponseMessage> httpRequstTask,
            TimeSpan timeout)
        {
            Task timeoutTask = Task.Delay(timeout);

            var result = await Task.WhenAny(timeoutTask, httpRequstTask);

            if (result == timeoutTask)
            {
                // Timeout!
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.RequestTimeout,
                };
            }

            return httpRequstTask.Result;
        }
        public static async Task<T> WithTimeout<T>(
            this Task<T> httpRequstTask,
            TimeSpan timeout)
        {
            Task timeoutTask = Task.Delay(timeout);

            var result = await Task.WhenAny(timeoutTask, httpRequstTask);

            if (result == timeoutTask)
            {
                // Timeout!
                return default;
            }

            return httpRequstTask.Result;
        }
    }
}

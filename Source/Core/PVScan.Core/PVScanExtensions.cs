using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PVScan.Core
{
    public static class PVScanExtensions
    {
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

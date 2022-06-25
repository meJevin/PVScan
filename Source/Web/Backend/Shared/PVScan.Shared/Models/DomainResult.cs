using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Shared.Models
{
    public class DomainResult
    {
        public bool Success { get; set; }

        public string? ErrorMessage { get; set; }

        public HttpStatusCode? StatusCode { get; set; }

        public DomainResult()
        {

        }

        public DomainResult(
            HttpStatusCode status, 
            string? errorMessage = null, 
            bool? success = null)
        {
            StatusCode = status;
            ErrorMessage = errorMessage;

            Success = success.HasValue ? success.Value : IsStatusCodeSuccessful;
        }

        public bool IsStatusCodeSuccessful => 
            (StatusCode.HasValue) && 
            ((int)StatusCode >= 200) &&
            ((int)StatusCode < 300);
    }
}

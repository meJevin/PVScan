using PVScan.API.Contract.Shared;
using PVScan.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Shared.Extensions
{
    public static class BasicResponseExtensions
    {
        public static DomainResult ToDomainError<T>(this BaseResponse<T> response)
            where T : BaseResponse<T>, new()
        {
            return new DomainResult
            {
                Success = response.Success,
                ErrorMessage = response.ErrorMessage,
                StatusCode = response.StatusCode,
            };
        }
    }
}

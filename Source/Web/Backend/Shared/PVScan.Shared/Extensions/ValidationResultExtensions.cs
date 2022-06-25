using FluentValidation.Results;
using PVScan.API.Contract.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Shared.Extensions
{
    public static class ValidationResultExtensions
    {
        public static ICollection<ApiError> ToApiErrors(this ICollection<ValidationFailure> validaitionFailures)
        {
            return validaitionFailures.Select(f => new ApiError
            {
                Message = "Validation Error",
                Details = f.ErrorMessage
            }).ToList();
        }
    }
}

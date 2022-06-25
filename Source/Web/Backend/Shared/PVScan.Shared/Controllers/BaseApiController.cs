using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PVScan.API.Contract.Shared.Models;
using PVScan.Shared.Extensions;
using PVScan.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Shared.Controllers
{
    public class BaseApiController : ControllerBase
    {
        protected IActionResult HandleError(DomainResult response)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.Forbidden:
                    return Forbidden(response.ErrorMessage);
                case HttpStatusCode.BadRequest:
                    return BadRequest(response.ErrorMessage);
                case HttpStatusCode.InternalServerError:
                    return InternalError(response.ErrorMessage);
                case HttpStatusCode.NotFound:
                    return NotFound(response.ErrorMessage);
            }

            throw new ArgumentOutOfRangeException();
        }

        protected BadRequestObjectResult BadRequest(ValidationResult result)
        {
            var apiErrors = result.Errors.ToApiErrors();

            var apiError = new ApiErrors
            {
                Errors = apiErrors
            };

            return new BadRequestObjectResult(apiError);
        }

        protected ObjectResult NotFound(object? error = null, string entityName = "Object")
        {
            var apiError = GenerateError(error?.ToString() ?? "", $"{entityName} Not Found");

            return StatusCode(StatusCodes.Status404NotFound, apiError);
        }

        protected new BadRequestObjectResult BadRequest(object? error)
        {
            var apiError = GenerateError(error, "Incorrect Payload");

            return new BadRequestObjectResult(apiError);
        }

        protected IActionResult Forbidden(object? body)
        {
            var apiError = GenerateError(body, "Authorization Failed");

            return StatusCode(StatusCodes.Status403Forbidden, apiError);
        }

        protected IActionResult InternalError(string? error)
        {
            var apiError = GenerateError(error, "Server Error");

            return StatusCode(StatusCodes.Status500InternalServerError, apiError);
        }

        protected IActionResult OkResult(object? responseBody = null)
        {
            if (responseBody is null)
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }

            return StatusCode(StatusCodes.Status200OK, responseBody);
        }

        protected IActionResult Created(object? responseBody = null)
        {
            if (responseBody is null)
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }

            return StatusCode(StatusCodes.Status201Created, responseBody);
        }

        private static ApiErrors GenerateError(object? error, string errorName)
        {
            return new ApiErrors
            {
                Errors = new List<ApiError>
                {
                    new ApiError
                    {
                        Message = errorName,
                        Details = error
                    }
                }
            };
        }
    }
}

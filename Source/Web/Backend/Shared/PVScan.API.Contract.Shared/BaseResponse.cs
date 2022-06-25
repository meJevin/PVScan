using System.Net;
using System.Text.Json.Serialization;

namespace PVScan.API.Contract.Shared
{
    public class BaseResponse<T> where T : BaseResponse<T>, new()
    {
        [JsonIgnore]
        public bool Success { get; set; } = true;

        [JsonIgnore]
        public string? ErrorMessage { get; set; }

        [JsonIgnore]
        public HttpStatusCode? StatusCode { get; set; }

        public static T InternalError(string error)
        {
            return new T
            {
                Success = false,
                ErrorMessage = error,
                StatusCode = HttpStatusCode.InternalServerError,
            };
        }

        public static T BadRequest(string error)
        {
            return new T
            {
                Success = false,
                ErrorMessage = error,
                StatusCode = HttpStatusCode.BadRequest
            };
        }

        public static T Forbidden(string error)
        {
            return new T
            {
                Success = false,
                ErrorMessage = error,
                StatusCode = HttpStatusCode.Forbidden
            };
        }

        public static T NotFound(string error)
        {
            return new T
            {
                Success = false,
                ErrorMessage = error,
                StatusCode = HttpStatusCode.NotFound
            };
        }

        public static T Ok()
        {
            return new T
            {
                Success = true,
                StatusCode = HttpStatusCode.OK
            };
        }
    }
}
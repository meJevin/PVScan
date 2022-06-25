using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace PVScan.API.Contract.Shared
{
    // Todo: put something here in the future :)
    public class BaseRequest
    {
        [JsonIgnore]
        public HttpContext? HttpContext { get; private set; }

        [JsonIgnore]
        public bool IsAuthenticated =>
            (HttpContext is not null) &&
            (HttpContext.User is not null) &&
            (HttpContext.User.Identity is not null) &&
            (HttpContext.User.Identity.IsAuthenticated);

        public BaseRequest()
        {
        }

        public void SetHttpContext(HttpContext context)
        {
            HttpContext = context;
        }
    }
}
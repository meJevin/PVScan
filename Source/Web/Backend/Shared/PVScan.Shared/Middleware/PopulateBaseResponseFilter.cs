using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using PVScan.API.Contract.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Shared.Middleware
{
    public class PopulateBaseResponseFilter : IAsyncActionFilter
    {
        public PopulateBaseResponseFilter()
        {
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var baseRequest = context.ActionArguments.Values.Where(a => a is BaseRequest).FirstOrDefault() as BaseRequest;

            if (baseRequest is null) return;

            baseRequest.SetHttpContext(context.HttpContext);

            await next();
        }
    }
}

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PVScan.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Shared.Services
{
    public class PVScanJwtBearerEvents : JwtBearerEvents
    {
        private ILogger<PVScanJwtBearerEvents> _logger;

        private void InjectDependencies(IServiceProvider provider)
        {
            _logger = provider.GetRequiredService<ILogger<PVScanJwtBearerEvents>>();
        }

        public override async Task MessageReceived(MessageReceivedContext context)
        {
            InjectDependencies(context.HttpContext.RequestServices);

            var headerTokenFound = context.Request.Headers
                .TryGetValue(Auth.AccessTokenHeaderName, out var tokenFromHeaders);

            if (headerTokenFound && !string.IsNullOrEmpty(tokenFromHeaders))
            {
                context.Token = tokenFromHeaders;
                return;
            }

            var queryTokenFound = context.Request.Query
                .TryGetValue(Auth.AccessTokenQueryVariableName, out var tokenFromQuery);

            if (queryTokenFound && !string.IsNullOrEmpty(tokenFromQuery))
            {
                context.Token = tokenFromHeaders;
                return;
            }
        }
    }
}

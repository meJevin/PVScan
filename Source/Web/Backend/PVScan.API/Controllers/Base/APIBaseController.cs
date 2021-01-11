using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PVScan.API.Controllers.Base
{
    [ApiController]
    public class APIBaseController : ControllerBase
    {
        protected string UserId =>
            HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value)
                .FirstOrDefault();
    }
}

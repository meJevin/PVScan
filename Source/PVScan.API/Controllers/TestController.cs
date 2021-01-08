using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PVScan.API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TestController : ControllerBase
    {
        public TestController()
        {
        }

        [HttpGet]
        public string NotSecret()
        {
            return "Hi";
        }

        [HttpGet]
        [Authorize("Default")]
        public string Secret()
        {
            return "Nigga";
        }
    }
}

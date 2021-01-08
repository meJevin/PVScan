using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PVScan.API.Controllers
{
    [Route("[controller]/[action]")]
    public class TestController : Controller
    {
        public TestController()
        {
        }

        public string NotSecret()
        {
            return "Hi";
        }

        [Authorize]
        public string Secret()
        {
            return "Nigga";
        }
    }
}

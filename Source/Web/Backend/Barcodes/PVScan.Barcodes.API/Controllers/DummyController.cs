using Microsoft.AspNetCore.Mvc;
using PVScan.Shared;

namespace PVScan.Barcodes.API.Controllers
{
    [ApiController]
    [Route("v{version:apiVersion}")]
    [ApiVersion("1.0")]
    public class DummyController : ControllerBase
    {
        public DummyController()
        {
        }

        [HttpGet]
        public async Task<IActionResult> Test()
        {
            return Ok("Dummy!");
        }
    }
}
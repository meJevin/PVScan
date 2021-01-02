using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PVScan.API.Models.Requests;
using PVScan.API.Services;
using PVScan.Domain.Models;
using PVScan.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PVScan.API.Controllers
{
    [Route("api/barcode")]
    [ApiController]
    public class BarcodeController : ControllerBase
    {
        readonly IBarcodeService barcodeService;

        public BarcodeController(IBarcodeService barcodeService, IFileStorage fileStorage)
        {
            this.barcodeService = barcodeService;
        }
    }
}

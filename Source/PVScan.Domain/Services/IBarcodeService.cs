using PVScan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Domain.Services
{
    public interface IBarcodeService
    {
        Task<Barcode> Create(Barcode barcode);
    }
}

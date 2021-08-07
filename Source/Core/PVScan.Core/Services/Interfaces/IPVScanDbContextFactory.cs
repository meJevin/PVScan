using PVScan.Core.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace PVScan.Core.Services.Interfaces
{
    public interface IPVScanDbContextFactory
    {
        PVScanDbContext Get();
    }
}

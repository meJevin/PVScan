using PVScan.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Core.Services.Interfaces
{
    public interface IBarcodeSynchronizer
    {
        event EventHandler SynchorinizedLocally;
        Task Synchronize(); 
    }
}

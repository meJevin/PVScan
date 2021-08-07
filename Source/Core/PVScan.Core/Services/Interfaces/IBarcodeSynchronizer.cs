using PVScan.Core.Models;
using PVScan.Core.Models.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Core.Services.Interfaces
{
    public interface IBarcodeSynchronizer
    {
        event EventHandler Started;
        event EventHandler<SynchronizeResponse> Finished;

        bool IsSynchronizing { get; }

        Task Synchronize(); 
    }
}

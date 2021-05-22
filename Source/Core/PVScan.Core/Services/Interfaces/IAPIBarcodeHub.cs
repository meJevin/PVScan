using Microsoft.AspNetCore.SignalR.Client;
using PVScan.Core.Models;
using PVScan.Core.Models.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Core.Services.Interfaces
{
    public interface IAPIBarcodeHub
    {
        Task Connect();
        Task Disconnect();

        Task Scanned(ScannedBarcodeRequest b);
        Task Deleted(DeletedBarcodeRequest b);
        Task Updated(UpdatedBarcodeRequest b);

        event EventHandler<ScannedBarcodeRequest> OnScanned;
        event EventHandler<DeletedBarcodeRequest> OnDeleted;
        event EventHandler<UpdatedBarcodeRequest> OnUpdated;
    }
}

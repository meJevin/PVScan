using Microsoft.AspNetCore.SignalR.Client;
using PVScan.Mobile.Models;
using PVScan.Mobile.Models.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Mobile.Services.Interfaces
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

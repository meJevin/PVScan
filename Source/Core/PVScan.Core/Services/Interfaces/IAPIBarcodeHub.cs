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
        Task ScannedMultiple(List<ScannedBarcodeRequest> reqs);
        Task Deleted(DeletedBarcodeRequest b);
        Task DeletedMultiple(List<DeletedBarcodeRequest> reqs);
        Task Updated(UpdatedBarcodeRequest b);
        Task UpdatedMultiple(List<UpdatedBarcodeRequest> reqs);

        event EventHandler<ScannedBarcodeRequest> OnScanned;
        event EventHandler<DeletedBarcodeRequest> OnDeleted;
        event EventHandler<UpdatedBarcodeRequest> OnUpdated;
        event EventHandler<List<ScannedBarcodeRequest>> OnScannedMultiple;
        event EventHandler<List<DeletedBarcodeRequest>> OnDeletedMultiple;
        event EventHandler<List<UpdatedBarcodeRequest>> OnUpdatedMultple;
    }
}

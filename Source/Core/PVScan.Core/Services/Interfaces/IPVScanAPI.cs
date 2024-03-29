﻿using PVScan.Core.Models;
using PVScan.Core.Models.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Core.Services.Interfaces
{
    public interface IPVScanAPI
    {
        Task<GetUserInfoResponse> GetUserInfo(GetUserInfoRequest req);
        Task<ChangeUserInfoResponse> ChangeUserInfo(ChangeUserInfoRequest req);

        Task<ScannedBarcodeResponse> ScannedBarcode(ScannedBarcodeRequest req);
        Task<ScannedBarcodeResponse> ScannedBarcodeMultiple(List<ScannedBarcodeRequest> reqs);
        Task<UpdatedBarcodeResponse> UpdatedBarcode(UpdatedBarcodeRequest req);
        Task<UpdatedBarcodeResponse> UpdatedBarcodeMultiple(List<UpdatedBarcodeRequest> reqs);
        Task<DeletedBarcodeRequest> DeletedBarcode(DeletedBarcodeRequest req);
        Task<DeletedBarcodeRequest> DeletedBarcodeMultiple(List<DeletedBarcodeRequest> reqs);

        Task<SynchronizeResponse> Synchronize(SynchronizeRequest req);
    }
}

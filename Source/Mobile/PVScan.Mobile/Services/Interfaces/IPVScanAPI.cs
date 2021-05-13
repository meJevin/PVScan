using PVScan.Mobile.Models;
using PVScan.Mobile.Models.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Mobile.Services.Interfaces
{
    public interface IPVScanAPI
    {
        Task<GetUserInfoResponse> GetUserInfo(GetUserInfoRequest req);
        Task<ChangeUserInfoResponse> ChangeUserInfo(ChangeUserInfoRequest req);

        Task<ScannedBarcodeResponse> ScannedBarcode(ScannedBarcodeRequest req);
        Task<UpdatedBarcodeResponse> UpdatedBarcode(UpdatedBarcodeRequest req);
        Task<DeletedBarcodeRequest> DeletedBarcode(DeletedBarcodeRequest req);
    }
}

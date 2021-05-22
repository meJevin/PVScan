using PVScan.Mobile.Models.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Mobile.Services.Interfaces
{
    public interface IAPIUserInfoHub
    {
        Task Connect();
        Task Disconnect();

        Task Changed(GetUserInfoResponse res);

        event EventHandler<GetUserInfoResponse> OnChanged;
    }
}

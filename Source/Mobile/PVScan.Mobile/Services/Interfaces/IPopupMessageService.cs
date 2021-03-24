using System;
using System.Threading.Tasks;

namespace PVScan.Mobile.Services.Interfaces
{
    public interface IPopupMessageService
    {
        Task ShowMessage(string messageText);
    }
}

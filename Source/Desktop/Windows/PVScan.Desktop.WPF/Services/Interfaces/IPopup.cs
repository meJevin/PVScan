using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Desktop.WPF.Services.Interfaces
{
    // Popup that is shown and returns result T
    public interface IPopup<T>
    {
        Task<T> ShowPopup();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Shared.Data
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
    }
}

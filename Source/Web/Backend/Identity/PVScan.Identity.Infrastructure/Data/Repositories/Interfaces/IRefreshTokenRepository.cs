using PVScan.Identity.Domain.Entities;
using PVScan.Shared.Data;

namespace PVScan.Identity.Infrastructure.Data.Repositories.Interfaces
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>, IUnitOfWork
    {
    }
}

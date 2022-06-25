using PVScan.Identity.Infrastructure.Data.Repositories.Interfaces;
using PVScan.Identity.Domain.Entities;
using PVScan.Shared.Data;

namespace PVScan.Identity.Infrastructure.Data.Repositories
{
    public class RefreshTokenRepository
        : EntityFrameworkRepository<RefreshToken, PVScanIdentityDbContext>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(PVScanIdentityDbContext dbContext)
            : base(dbContext)
        {

        }
    }
}

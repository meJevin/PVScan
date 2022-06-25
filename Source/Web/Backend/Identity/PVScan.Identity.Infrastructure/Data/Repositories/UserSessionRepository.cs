using PVScan.Identity.Infrastructure.Data.Repositories.Interfaces;
using PVScan.Identity.Domain.Entities;
using PVScan.Shared.Data;

namespace PVScan.Identity.Infrastructure.Data.Repositories
{
    public class UserSessionRepository 
        : EntityFrameworkRepository<UserSession, PVScanIdentityDbContext>, IUserSessionRepository
    {
        public UserSessionRepository(PVScanIdentityDbContext dbContext)
            : base(dbContext)
        {

        }
    }
}

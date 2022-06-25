using PVScan.Identity.Infrastructure.Data.Repositories.Interfaces;
using PVScan.Identity.Domain.Entities;
using PVScan.Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Identity.Infrastructure.Data.Repositories
{
    public class UserInfoRepository
        : EntityFrameworkRepository<UserInfo, PVScanIdentityDbContext>, IUserInfoRepository
    {
        public UserInfoRepository(PVScanIdentityDbContext dbContext) 
            : base(dbContext)
        {

        }
    }
}

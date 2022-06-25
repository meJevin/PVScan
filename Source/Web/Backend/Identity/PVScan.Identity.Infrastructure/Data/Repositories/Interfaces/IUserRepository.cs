using Microsoft.AspNetCore.Identity;
using PVScan.Identity.Domain.Entities;
using PVScan.Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Identity.Infrastructure.Data.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>, IUnitOfWork
    {
        Task<IdentityResult> CreateWithPassword(User user, string password);
    }
}

using Microsoft.EntityFrameworkCore;
using PVScan.Domain.Models;
using PVScan.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.EntityFramework.Services
{
    public class EFUserService : IUserService
    {
        readonly PVScanDbContext context;

        public Task<User> CreateUser(User user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUser(User user)
        {
            throw new NotImplementedException();
        }
    }
}

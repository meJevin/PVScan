using PVScan.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Domain.Services
{
    public interface IUserService
    {
        Task<User> CreateUser(User user);
        Task DeleteUser(User user);
    }
}

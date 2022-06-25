using PVScan.Identity.Infrastructure.Data.Repositories.Interfaces;
using PVScan.Identity.Domain.Entities;
using PVScan.Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PVScan.Identity.Infrastructure.Data.Repositories
{
    public class UserRepository 
        : EntityFrameworkRepository<User, PVScanIdentityDbContext>, IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserInfoRepository _userInfoRepository;

        public UserRepository(
            PVScanIdentityDbContext dbContext,
            UserManager<User> userManager,
            SignInManager<User> signInManager, 
            IUserInfoRepository userInfoRepository)
            : base(dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userInfoRepository = userInfoRepository;
        }

        public async Task<IdentityResult> CreateWithPassword(User user, string password)
        {
            var userInfo = new UserInfo { Id = Guid.NewGuid(), User = user };
            await _userInfoRepository.AddAsync(userInfo);

            user.Info = userInfo;
            user.UserInfoId = userInfo.Id;

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(user, password, true, true);
            }

            return result;
        }
    }
}

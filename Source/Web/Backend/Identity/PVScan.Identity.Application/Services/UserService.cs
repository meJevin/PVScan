using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PVScan.Identity.Application.Services.Interfaces;
using PVScan.Identity.Domain.Entities;
using PVScan.Identity.Infrastructure.Data.Repositories.Interfaces;
using PVScan.Shared.Data;
using PVScan.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Identity.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly SignInManager<User> _signInManager;

        public UserService(
            IUserRepository userRepository, 
            SignInManager<User> signInManager)
        {
            _userRepository = userRepository;
            _signInManager = signInManager;
        }

        public async Task<(User? User, DomainResult Result)> LoginWithUsernameAndPassword(LoginUsernameAndPasswordData data)
        {
            var found = await _userRepository.Query().FirstOrDefaultAsync(a => a.UserName == data.Username);

            if (found is null)
            {
                return (null, new DomainResult(HttpStatusCode.NotFound, $"Could not find user with username {data.Username}"));
            }

            var result = await _signInManager.CheckPasswordSignInAsync(found, data.Password, true);

            if (!result.Succeeded)
            {
                return (null, new DomainResult(HttpStatusCode.Forbidden, $"Password is not correct"));
            }

            return (found, new DomainResult(HttpStatusCode.OK));
        }

        public async Task<(User? User, DomainResult Result)> RegisterNewAsync(RegisterNewUserData data)
        {
            var newUser = new User { UserName = data.Username, Email = data.Email };

            var creationResult = await _userRepository.CreateWithPassword(newUser, data.Password);

            if (!creationResult.Succeeded)
            {
                return new(null, new DomainResult(HttpStatusCode.BadRequest, 
                    String.Join(", ", creationResult.Errors.Select(a => a.Description))));
            }

            return new(newUser, new DomainResult(HttpStatusCode.OK));
        }
    }
}

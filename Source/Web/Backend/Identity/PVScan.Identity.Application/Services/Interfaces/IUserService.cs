﻿using PVScan.Identity.Domain.Entities;
using PVScan.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Identity.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<DomainResult> Logout(LogoutData data);
        Task<(User? User, DomainResult Result)> LoginWithUsernameAndPassword(LoginUsernameAndPasswordData data);
        Task<(User? User, DomainResult Result)> RegisterNewAsync(RegisterNewUserData data);
    }

    public record LogoutData(User CurrentUser, UserSession? CurrentSession = null, string? RefreshToken = null);
    public record RegisterNewUserData(string Username, string Password, string Email);
    public record LoginUsernameAndPasswordData(string Username, string Password);
}
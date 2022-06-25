using PVScan.Identity.Domain.Entities;
using PVScan.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Identity.Application.Services.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<(RefreshToken? Token, DomainResult Result)> Revoke(RefreshToken token, RevokeData data);
        Task<(RefreshToken? Token, DomainResult Result)> RevokeByValue(string refreshToken, RevokeData data);
    }

    public record RevokeData(string RevokeReason);
}

using Microsoft.EntityFrameworkCore;
using PVScan.Identity.Application.Services.Interfaces;
using PVScan.Identity.Domain.Entities;
using PVScan.Identity.Infrastructure.Data.Repositories.Interfaces;
using PVScan.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Identity.Application.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<(RefreshToken? Token, DomainResult Result)> Revoke(RefreshToken token, RevokeData data)
        {
            if (token.IsRevoked)
            {
                return (null, new DomainResult(HttpStatusCode.BadRequest));
            }

            token.Revoked = DateTime.UtcNow;
            token.ReasonRevoked = data.RevokeReason;

            await _refreshTokenRepository.UpdateAsync(token);
            await _refreshTokenRepository.CommitAsync();

            return (token, new DomainResult(HttpStatusCode.OK));
        }

        public async Task<(RefreshToken? Token, DomainResult Result)> RevokeByValue(string refreshToken, RevokeData data)
        {
            var tokenFound = await _refreshTokenRepository.Query().FirstOrDefaultAsync(a => a.Token == refreshToken);

            if (tokenFound is null)
            {
                return (null, new DomainResult(HttpStatusCode.NotFound, 
                    $"Could not find refresh token with value {refreshToken}"));
            }

            return await Revoke(tokenFound, data);
        }
    }
}

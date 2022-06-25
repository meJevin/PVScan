using Microsoft.AspNetCore.Identity;
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
    public class UserSessionService : IUserSessionService
    {
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public UserSessionService(
            IUserSessionRepository userSessionRepository, 
            IRefreshTokenRepository refreshTokenRepository)
        {
            _userSessionRepository = userSessionRepository;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<(UserSession? Session, DomainResult Result)> StartNewSession(StartNewSessionData data)
        {
            var sessionToCreate = new UserSession
            {
                Id = Guid.NewGuid(), 
                RefreshTokenId = data.RefreshToken.Id,
                UserId = data.User.Id
            };

            data.RefreshToken.CreatedByIp = data.FromIp;
            data.RefreshToken.UserSessionId = sessionToCreate.Id;

            await _refreshTokenRepository.UpdateAsync(data.RefreshToken);
            await _userSessionRepository.AddAsync(sessionToCreate);
            await _userSessionRepository.CommitAsync();

            return (sessionToCreate, new DomainResult(HttpStatusCode.OK));
        }

        public async Task<DomainResult> TerminateSession(TerminateSessionData data)
        {
            var foundSession = await _userSessionRepository.Query()
                .FirstOrDefaultAsync(a => a.RefreshTokenId == data.RefreshToken.Id);

            if (foundSession is null)
            {
                return new DomainResult(HttpStatusCode.NotFound, 
                    $"Could not find user session for refresh token {data.RefreshToken.Token}");
            }

            await _userSessionRepository.DeleteAsync(foundSession);
            await _userSessionRepository.CommitAsync();

            return new DomainResult(HttpStatusCode.OK);
        }

        public async Task<DomainResult> TerminateSession(UserSession session)
        {
            await _userSessionRepository.DeleteAsync(session);
            await _userSessionRepository.CommitAsync();

            return new DomainResult(HttpStatusCode.OK);
        }
    }
}

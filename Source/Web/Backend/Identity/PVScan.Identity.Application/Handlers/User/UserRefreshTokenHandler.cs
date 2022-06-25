using MediatR;
using Microsoft.EntityFrameworkCore;
using PVScan.Identity.API.Contract.Requests.User;
using PVScan.Identity.API.Contract.Responses.Token;
using PVScan.Identity.API.Contract.Responses.User;
using PVScan.Identity.Application.Services.Interfaces;
using PVScan.Identity.Infrastructure.Data.Repositories.Interfaces;
using PVScan.Identity.Infrastructure.Services;

namespace PVScan.Identity.Application.Handlers.User
{
    public class UserRefreshTokenHandler : IRequestHandler<UserRefreshTokenRequest, UserRefreshTokenResponse>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenFactory _jwtTokenFactory;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IUserSessionRepository _userSessionRepository;

        public UserRefreshTokenHandler(
            IRefreshTokenRepository refreshTokenRepository,
            IUserRepository userRepository,
            IJwtTokenFactory jwtTokenFactory,
            IRefreshTokenService refreshTokenService, 
            IUserSessionRepository userSessionRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _userRepository = userRepository;
            _jwtTokenFactory = jwtTokenFactory;
            _refreshTokenService = refreshTokenService;
            _userSessionRepository = userSessionRepository;
        }

        public async Task<UserRefreshTokenResponse> Handle(
            UserRefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var refreshToken = await _refreshTokenRepository
                .Query()
                .Include(a => a.UserSession)
                .FirstOrDefaultAsync(r => r.Token == request.RefreshToken);

            if (refreshToken is null)
            {
                return UserRefreshTokenResponse.NotFound($"Could not find token with value {request.RefreshToken}");
            }

            if (refreshToken.IsExpired)
            {
                return UserRefreshTokenResponse.Forbidden($"Token with value {request.RefreshToken} is expired");
            }

            if (refreshToken.IsRevoked)
            {
                return UserRefreshTokenResponse.Forbidden($"Token with value {request.RefreshToken} has been revoked");
            }

            if (refreshToken.UserSession is null)
            {
                return UserRefreshTokenResponse.InternalError($"Refresh token {request.RefreshToken} has no user session");
            }

            var userSession = refreshToken.UserSession;

            var refreshTokenUserId = refreshToken.UserSession.UserId;
            var user = await _userRepository.Query().FirstOrDefaultAsync(a => a.Id == refreshTokenUserId);

            if (user is null)
            {
                return UserRefreshTokenResponse.InternalError($"Could not link refresh token {request.RefreshToken} with user {refreshTokenUserId}");
            }

            var newAccessToken = await _jwtTokenFactory.GenerateAccessTokenAsync(new AccessTokenGenerationData(user));
            var newRefreshToken = await _jwtTokenFactory.GenerateRefreshTokenAsync(new RefreshTokenGenerationData());

            userSession.RefreshToken = newRefreshToken;
            userSession.RefreshTokenId = newRefreshToken.Id;

            await _userSessionRepository.UpdateAsync(userSession);

            await _refreshTokenService.Revoke(refreshToken, new("Token Refresh"));

            return new UserRefreshTokenResponse()
            {
                NewAccessToken = new TokenResponse(newAccessToken.Token, newAccessToken.Expires),
                NewRefreshToken = new TokenResponse(newRefreshToken.Token, newRefreshToken.Expires),
            };
        }
    }
}

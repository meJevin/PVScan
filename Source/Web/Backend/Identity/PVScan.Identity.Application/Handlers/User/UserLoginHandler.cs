using MediatR;
using PVScan.Identity.API.Contract.Requests.User;
using PVScan.Identity.API.Contract.Responses.Token;
using PVScan.Identity.API.Contract.Responses.User;
using PVScan.Identity.Application.Services.Interfaces;
using PVScan.Identity.Infrastructure.Data.Repositories.Interfaces;
using PVScan.Identity.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Identity.Application.Handlers.User
{
    public class UserLoginHandler : IRequestHandler<UserLoginRequest, UserLoginResponse>
    {
        private readonly IUserService _userService;
        private readonly IJwtTokenFactory _jwtTokenFactory;
        private readonly IUserSessionService _userSessionService;

        public UserLoginHandler(
            IUserService userService,
            IJwtTokenFactory jwtTokenFactory, 
            IUserSessionService userSessionService)
        {
            _userService = userService;
            _jwtTokenFactory = jwtTokenFactory;
            _userSessionService = userSessionService;
        }

        public async Task<UserLoginResponse> Handle(
            UserLoginRequest request, CancellationToken cancellationToken)
        {
            var loginData = new LoginUsernameAndPasswordData(request.Username, request.Password);
            var (user, loginResult) = await _userService.LoginWithUsernameAndPassword(loginData);

            if (!loginResult.Success || user is null)
            {
                return UserLoginResponse.Forbidden($"Could not login with username and password. Error: {loginResult.ErrorMessage}");
            }

            var accessToken = await _jwtTokenFactory.GenerateAccessTokenAsync(new AccessTokenGenerationData(user));
            var refreshToken = await _jwtTokenFactory.GenerateRefreshTokenAsync(new RefreshTokenGenerationData());

            var remoteIp = request.HttpContext?.Connection.RemoteIpAddress?.ToString();

            await _userSessionService.StartNewSession(new(user, refreshToken, remoteIp));

            return new UserLoginResponse
            {
                AccessToken = new TokenResponse(accessToken.Token, accessToken.Expires),
                RefreshToken = new TokenResponse(refreshToken.Token, refreshToken.Expires),
            };
        }
    }
}

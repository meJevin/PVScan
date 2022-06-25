using MediatR;
using PVScan.Identity.API.Contract.Requests.User;
using PVScan.Identity.API.Contract.Responses.User;
using PVScan.Identity.Application.Services.Interfaces;
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

        public UserLoginHandler(
            IUserService userService,
            IJwtTokenFactory jwtTokenFactory)
        {
            _userService = userService;
            _jwtTokenFactory = jwtTokenFactory;
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

            return new UserLoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
            };
        }
    }
}

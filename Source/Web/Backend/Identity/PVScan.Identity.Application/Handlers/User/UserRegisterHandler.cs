using MediatR;
using PVScan.Identity.API.Contract.Requests.User;
using PVScan.Identity.API.Contract.Responses.Token;
using PVScan.Identity.API.Contract.Responses.User;
using PVScan.Identity.Application.Services.Interfaces;
using PVScan.Identity.Infrastructure.Services;

namespace PVScan.Identity.Application.Handlers.User
{
    public class UserRegisterHandler : IRequestHandler<UserRegisterRequest, UserRegisterResponse>
    {
        private readonly IUserService _userService;
        private readonly IJwtTokenFactory _jwtTokenFactory;
        private readonly IUserSessionService _userSessionService;

        public UserRegisterHandler(
            IUserService userService,
            IJwtTokenFactory jwtTokenFactory, 
            IUserSessionService userSessionService)
        {
            _userService = userService;
            _jwtTokenFactory = jwtTokenFactory;
            _userSessionService = userSessionService;
        }

        public async Task<UserRegisterResponse> Handle(
            UserRegisterRequest request, CancellationToken cancellationToken)
        {
            var registrationData = new RegisterNewUserData(request.Username, request.Password, request.Email);
            var (newUser, registrationResult) = await _userService.RegisterNewAsync(registrationData);

            if (!registrationResult.Success || newUser is null)
            {
                return UserRegisterResponse.InternalError($"Could not register new user. Error: {registrationResult.ErrorMessage}");
            }

            var accessToken = await _jwtTokenFactory.GenerateAccessTokenAsync(new AccessTokenGenerationData(newUser));
            var refreshToken = await _jwtTokenFactory.GenerateRefreshTokenAsync(new RefreshTokenGenerationData());

            var remoteIp = request.HttpContext?.Connection.RemoteIpAddress?.ToString();

            await _userSessionService.StartNewSession(new(newUser, refreshToken, remoteIp));

            return new UserRegisterResponse
            {
                AccessToken = new TokenResponse(accessToken.Token, accessToken.Expires),
                RefreshToken = new TokenResponse(refreshToken.Token, refreshToken.Expires),
            };
        }
    }
}

using MediatR;
using PVScan.Identity.API.Contract.Requests.User;
using PVScan.Identity.API.Contract.Responses.User;
using PVScan.Identity.Application.Services.Interfaces;
using PVScan.Identity.Infrastructure.Services;

namespace PVScan.Identity.Application.Handlers.User
{
    public class UserRegisterHandler : IRequestHandler<UserRegisterRequest, UserRegisterResponse>
    {
        private readonly IUserService _userService;
        private readonly IJwtTokenFactory _jwtTokenFactory;

        public UserRegisterHandler(
            IUserService userService, 
            IJwtTokenFactory jwtTokenFactory)
        {
            _userService = userService;
            _jwtTokenFactory = jwtTokenFactory;
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

            return new UserRegisterResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
            };
        }
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using PVScan.Identity.API.Contract.Requests.User;
using PVScan.Identity.API.Contract.Responses.Token;
using PVScan.Identity.API.Contract.Responses.User;
using PVScan.Identity.Application.Services.Interfaces;
using PVScan.Identity.Infrastructure.Data.Repositories.Interfaces;
using PVScan.Identity.Infrastructure.Services;
using System.IdentityModel.Tokens.Jwt;

namespace PVScan.Identity.Application.Handlers.User
{
    public class UserLogoutHandler : IRequestHandler<UserLogoutRequest, UserLogoutResponse>
    {
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        private readonly IUserSessionRepository _userSessionRepository;

        public UserLogoutHandler(
            IUserService userService,
            IUserRepository userRepository,
            IUserSessionRepository userSessionRepository, 
            IUserSessionService userSessionService)
        {
            _userService = userService;
            _userRepository = userRepository;
            _userSessionRepository = userSessionRepository;
        }

        public async Task<UserLogoutResponse> Handle(
            UserLogoutRequest request, CancellationToken cancellationToken)
        {
            if (request.HttpContext?.User is null)
            {
                return UserLogoutResponse.Forbidden($"Can't logout because user is not authenticated");
            }

            var currentUserId = request.HttpContext.User.Claims
                .FirstOrDefault(a => a.Type == "user_id")?.Value;

            if (currentUserId is null)
            {
                return UserLogoutResponse.BadRequest($"Could not find unique name claim for current user");
            }

            var currentUser = await _userRepository.Query().FirstOrDefaultAsync(a => a.Id.ToString() == currentUserId);

            if (currentUser is null)
            {
                return UserLogoutResponse.NotFound($"Could not find user with id {currentUserId}");
            }

            var currentUserSession = await _userSessionRepository.Query()
                .Include(a => a.RefreshToken)
                .FirstOrDefaultAsync(a => 
                    a.RefreshToken != null && 
                    a.RefreshToken.Token == request.RefreshToken);

            var logoutData = new LogoutData(currentUser, currentUserSession, request.RefreshToken);
            var logoutResult = await _userService.Logout(logoutData);

            if (!logoutResult.Success)
            {
                return UserLogoutResponse.InternalError($"Could not logout. Error: {logoutResult.ErrorMessage}");
            }

            return UserLogoutResponse.Ok();
        }
    }
}

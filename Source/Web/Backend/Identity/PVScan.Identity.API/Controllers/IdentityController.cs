using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PVScan.Identity.API.Contract.Requests.User;
using PVScan.Shared.Controllers;
using PVScan.Shared.Extensions;

namespace PVScan.Identity.API.Controllers
{
    [ApiController]
    [Route("v{version:apiVersion}")]
    [ApiVersion("1.0")]
    [Authorize]
    public class IdentityController : BaseApiController
    {
        private readonly IMediator _mediator;

        public IdentityController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login(UserLoginRequest request)
        {
            var result = await _mediator.Send(request);

            if (!result.Success)
            {
                return HandleError(result.ToDomainError());
            }

            return Ok(result);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<IActionResult> Register(UserRegisterRequest request)
        {
            var result = await _mediator.Send(request);

            if (!result.Success)
            {
                return HandleError(result.ToDomainError());
            }

            return Created(result);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(UserRefreshTokenRequest request)
        {
            var result = await _mediator.Send(request);

            if (!result.Success)
            {
                return HandleError(result.ToDomainError());
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout(UserLogoutRequest request)
        {
            var result = await _mediator.Send(request);

            if (!result.Success)
            {
                return HandleError(result.ToDomainError());
            }

            return Ok(result);
        }
    }
}
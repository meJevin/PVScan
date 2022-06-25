using MediatR;
using PVScan.API.Contract.Shared;
using PVScan.Identity.API.Contract.Responses.User;

namespace PVScan.Identity.API.Contract.Requests.User
{
    public class UserRefreshTokenRequest : BaseRequest, IRequest<UserRefreshTokenResponse>
    {
        public string RefreshToken { get; set; }
    }
}

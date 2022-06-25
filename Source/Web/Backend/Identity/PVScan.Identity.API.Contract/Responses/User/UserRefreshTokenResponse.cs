using PVScan.API.Contract.Shared;
using PVScan.Identity.API.Contract.Responses.Token;

namespace PVScan.Identity.API.Contract.Responses.User
{
    public class UserRefreshTokenResponse : BaseResponse<UserRefreshTokenResponse>
    {
        public TokenResponse? NewAccessToken { get; set; }
        public TokenResponse? NewRefreshToken { get; set; }
    }
}

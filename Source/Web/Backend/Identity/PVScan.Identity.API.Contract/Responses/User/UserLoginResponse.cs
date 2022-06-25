using PVScan.API.Contract.Shared;
using PVScan.Identity.API.Contract.Responses.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Identity.API.Contract.Responses.User
{
    public class UserLoginResponse : BaseResponse<UserLoginResponse>
    {
        public TokenResponse? AccessToken { get; set; }
        public TokenResponse? RefreshToken { get; set; }
    }
}

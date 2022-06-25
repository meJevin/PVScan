using PVScan.API.Contract.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Identity.API.Contract.Responses.User
{
    public class UserLoginResponse : BaseResponse<UserLoginResponse>
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}

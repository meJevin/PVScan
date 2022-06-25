using MediatR;
using PVScan.API.Contract.Shared;
using PVScan.Identity.API.Contract.Responses.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Identity.API.Contract.Requests.User
{
    public class UserLoginRequest : BaseRequest, IRequest<UserLoginResponse>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

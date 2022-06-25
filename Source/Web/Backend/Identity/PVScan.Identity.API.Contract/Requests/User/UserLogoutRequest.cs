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
    public class UserLogoutRequest : BaseRequest, IRequest<UserLogoutResponse>
    {
        public string? RefreshToken { get; set; }
    }
}

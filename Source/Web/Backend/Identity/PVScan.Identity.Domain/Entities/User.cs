using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Identity.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public Guid UserInfoId { get; set; }
        public UserInfo? Info { get; set; }
        public IEnumerable<UserSession>? Sessions { get; set; }
    }
}

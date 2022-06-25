using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Identity.Domain.Entities
{
    public class UserSession
    {
        public Guid Id { get; set; }
        
        public Guid UserId { get; set; }
        public User? User { get; set; }

        public Guid RefreshTokenId { get; set; }
        public RefreshToken? RefreshToken { get; set; }
    }
}

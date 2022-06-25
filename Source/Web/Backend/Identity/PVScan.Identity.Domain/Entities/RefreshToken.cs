using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Identity.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }  
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public string? CreatedByIp { get; set; }
        public DateTime? Revoked { get; set; }
        public string? RevokedByIp { get; set; }
        public string? ReplacedByToken { get; set; }
        public string? ReasonRevoked { get; set; }
        public Guid? UserSessionId { get; set; }
        public UserSession? UserSession { get; set; }

        public RefreshToken(
            Guid id, string token,
            DateTime expires, DateTime created,
            string? createdByIp = null, DateTime? revoked = null,
            string? revokedByIp = null, string? replacedByToken = null,
            string? reasonRevoked = null, Guid? userSessionId = null)
        {
            Id = id;
            Token = token;
            Expires = expires;
            Created = created;
            CreatedByIp = createdByIp;
            Revoked = revoked;
            RevokedByIp = revokedByIp;
            ReplacedByToken = replacedByToken;
            ReasonRevoked = reasonRevoked;
            UserSessionId = userSessionId;
        }

        #region HELPERS
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public bool IsRevoked => Revoked != null;
        public bool IsActive => !IsRevoked && !IsExpired;
        #endregion
    }
}

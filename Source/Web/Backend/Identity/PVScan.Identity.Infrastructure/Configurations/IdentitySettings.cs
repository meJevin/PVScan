using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Identity.Infrastructure.Configurations
{
    public class IdentitySettings
    {
        public string PrivateKeyPath { get; set; }
        public TimeSpan AccessTokenLifetime { get; set; }
        public TimeSpan RefreshTokenLifetime { get; set; }
    }
}

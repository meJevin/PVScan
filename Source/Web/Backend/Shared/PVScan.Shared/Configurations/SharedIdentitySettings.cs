using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Shared.Configurations
{
    public class SharedIdentitySettings
    {
        public string PublicKeyPath { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
    }
}

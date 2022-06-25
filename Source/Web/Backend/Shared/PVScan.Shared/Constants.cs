using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Shared
{
    public static class Auth
    {
        public static string AccessTokenHeaderName => "x-pvscan-access-token";
        public static string AccessTokenQueryVariableName => "acceess_token";
    }
}

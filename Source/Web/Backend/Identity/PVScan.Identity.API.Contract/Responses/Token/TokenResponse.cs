using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Identity.API.Contract.Responses.Token
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }

        public TokenResponse(string token, DateTime expires)
        {
            Token = token;
            Expires = expires;
        }
    }
}

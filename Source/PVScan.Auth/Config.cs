using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PVScan.Auth
{
    public static class Config
    {
        public static List<TestUser> Users =>
            new List<TestUser>
            {
                new TestUser()
                {
                    Username = "test1",
                    Password = "test1Pass",
                    IsActive = true,
                    ProviderName = "Michael",
                }
            };

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };


        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("PVScan.API", "Main PVScan API")
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                // machine to machine clientnew Client
                new Client
                {
                    ClientId = "client",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    AllowedScopes = new List<string>
                    {
                        "PVScan.API"
                    }
                }
            };
    }
}

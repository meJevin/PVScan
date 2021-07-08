using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PVScan.Auth
{
    public static class Config
    {
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
                new Client {
                    ClientId = "PVScan.Auth.WPF",

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "PVScan.API",
                    },

                    AccessTokenLifetime = (int)TimeSpan.FromDays(30).TotalSeconds,
                    IdentityTokenLifetime = (int)TimeSpan.FromDays(30).TotalSeconds,

                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                },
                new Client {
                    ClientId = "PVScan.Auth.Mobile",

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "PVScan.API",
                    },

                    AccessTokenLifetime = (int)TimeSpan.FromDays(30).TotalSeconds,
                    IdentityTokenLifetime = (int)TimeSpan.FromDays(30).TotalSeconds,

                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                }
            };
    }
}

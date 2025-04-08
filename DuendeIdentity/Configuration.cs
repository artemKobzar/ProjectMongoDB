using DuendeIdentity.Models;
using Duende.IdentityServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using Duende.IdentityServer.Models;

namespace DuendeIdentity
{
    public static class Configuration
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource
                {
                    Name = "roles",
                    DisplayName = "Roles",
                    UserClaims = new List<string>
                    {
                        JwtClaimTypes.Role
                    }
                }
            };
        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("ProjectMongoDB", "Web API")
                {
                    UserClaims = new string [] { JwtClaimTypes.Role }
                }
            };
        public static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource>
            {
                new ApiResource("ProjectMongoDB")
                {
                    Scopes = { "ProjectMongoDB" },
                    UserClaims = new List<string> {"role"}
                }
            };
        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "interactive",
                    ClientSecrets = { new Secret("clientSecret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireClientSecret = false,
                    AllowedCorsOrigins = {"https://localhost:7153"},
                    RedirectUris = new List<string>{ "https://localhost:7153/signin-oidc","https://oauth.pstmn.io/v1/browser-callback" },
                    PostLogoutRedirectUris = { "https://localhost:7153/signout-callback-oidc" },
                    RequirePkce = true,
                    RequireConsent = true,
                    AllowOfflineAccess = true,
                    
                    AllowedScopes = new List<string>
                    {
                        "ProjectMongoDB",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "roles"
                    }
                }
            };
    }
}

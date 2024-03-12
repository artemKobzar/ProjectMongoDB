using IdentityServer4.Models;
using IdentityServer4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using static System.Net.WebRequestMethods;

namespace IdentityService4
{
    public static class Configuration
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
            };
        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("ProjectMongoDB", "Web API")
            };
        public static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource>
            {
                //new ApiResource("ProjectMongoDB")
                new ApiResource("ProjectMongoDB", "Web API", new [] {JwtClaimTypes.Name})
                {
                    Scopes = { "ProjectMongoDB" }
                }
            };
        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "project-test-api",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireClientSecret = false,
                    AllowedCorsOrigins = {"https://localhost:7253"},
                    RedirectUris = { "https://localhost:7253/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { "https://localhost:7253/signout-callback-oidc" },
                    RequirePkce = true,
                    RequireConsent = false,
                    AllowOfflineAccess = true,
                    
                    AllowedScopes = new List<string>
                    {
                        "ProjectMongoDB",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email
                    }
                }
            };
    }

}

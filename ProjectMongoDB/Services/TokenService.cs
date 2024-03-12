using IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectMongoDB.Services
{
    public class TokenService : ITokenService
    {
        public readonly IOptions<IdentityServerSettings> _identityServerSettings;
        public readonly DiscoveryDocumentResponse _discoveryDocumentResponse;
        private readonly HttpClient _httpClient;
        public TokenService(IOptions<IdentityServerSettings> identityServerSettings)
        {
            _identityServerSettings = identityServerSettings;
            _httpClient = new HttpClient();
            _discoveryDocumentResponse = _httpClient.GetDiscoveryDocumentAsync(_identityServerSettings.Value.DiscoveryUrl).Result;
            if(_discoveryDocumentResponse.IsError)
            {
                throw new Exception("Unable to get discoverydocument", _discoveryDocumentResponse.Exception);
            }
        }
        public async Task<TokenResponse> GetToken(string scope)
        {
            var tokenResponse = await _httpClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
            {
                Address = _discoveryDocumentResponse.TokenEndpoint,
                ClientId = _identityServerSettings.Value.ClientName,
                ClientSecret = _identityServerSettings.Value.ClientPassword
            });

            if(tokenResponse.IsError)
            {
                throw new Exception("Unable to get token", tokenResponse.Exception);
            }
            return tokenResponse;
        }
    }
}

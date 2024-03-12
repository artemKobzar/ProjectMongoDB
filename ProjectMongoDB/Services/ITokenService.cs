using IdentityModel.Client;

namespace ProjectMongoDB.Services
{
    public interface ITokenService
    {
        Task <TokenResponse> GetToken (string scope);
    }
}

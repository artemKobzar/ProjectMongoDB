using IdentityModel.Client;

namespace ClientMongo.Services
{
    public interface ITokenService
    {
        Task <TokenResponse> GetToken (string scope);
    }
}

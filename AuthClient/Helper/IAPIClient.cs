using System.Threading.Tasks;
using Thinktecture.IdentityModel.Client;

namespace AuthClient.Helper
{
    public interface IAPIClient
    {
        object GetToken(string userId, string password);//not required
        Task<TokenResponse> GetTokenNew(string userId, string password, string clientKey, string clientSecret);
        Task<TokenResponse> GetRefreshToken(string refreshToken, string clientKey, string clientSecret);
        Task<string> GetIdentity(string token);
        //string Validate(string token);
    }
}
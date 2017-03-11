using System.Threading.Tasks;

namespace ABC.Leaves.Api.Services
{
    public interface IGoogleOAuthService
    {
        string BuildOfflineAccessChallengeUrl(string redirectUrl, string state);
        Task<OAuthExchangeResult> ExchangeCode(string code, string redirectUrl);
        Task<OAuthExchangeResult> ExchangeRefreshToken(string refreshToken);
    }
}

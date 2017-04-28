using System.Threading.Tasks;

namespace AbcLeaves.BasicMvcClient.Domain
{
    public interface IGoogleApisAuthManager
    {
        Task<ReturnUrlResult> GetChallengeUrl(string redirectUrl,
            string returnUrl);
        Task<ReturnUrlResult> HandleOAuthExchangeCode(string code, string state, string error,
            string redirectUrl);
    }
}

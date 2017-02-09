using System.Threading.Tasks;
using ABC.Leaves.Api.GoogleAuth.Dto;

namespace ABC.Leaves.Api.GoogleAuth
{
    public interface IGoogleAuthService
    {
        GetAuthUrlResult GetAuthUrl(string redirectUrl);
        Task<GetAccessTokenResult> GetAccessTokenAsync(string code, string redirectUrl);
        Task<GetAccessTokenInfoResult> GetAccessTokenInfoAsync(string accessToken);
    }
}

using System.Threading.Tasks;
using ABC.Leaves.Api.GoogleAuth.Dto;

namespace ABC.Leaves.Api.GoogleAuth
{
    public interface IGoogleAuthService
    {
        GetAuthUrlOutput GetAuthUrl(GetAuthUrlInput input);
        Task<GetAccessTokenOutput> GetAccessTokenAsync(GetAccessTokenInput input);
        Task<GetAccessTokenInfoOutput> GetAccessTokenInfoAsync(GetAccessTokenInfoInput input);
    }
}

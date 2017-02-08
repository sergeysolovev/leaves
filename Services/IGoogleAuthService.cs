using System.Threading.Tasks;

namespace ABC.Leaves.Api.Services
{
    public interface IGoogleAuthService
    {
        string GetAuthUrl(string redirectUrl);
        Task<string> GetAccessTokenAsync(string code, string redirectUrl);
    }
}

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ABC.Leaves.Api.Services
{
    public interface IGoogleAuthService
    {
        string GetAuthUrl(string redirectUrl);
        Task<ObjectResult> GetAccessTokenAsync(string code, string redirectUrl);
    }
}

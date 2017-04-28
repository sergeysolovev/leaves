using System.Security.Claims;
using System.Threading.Tasks;
using AbcLeaves.Core;

namespace AbcLeaves.Api.Domain
{
    public interface IGoogleApisAuthManager
    {
        Task<VerifyAccessResult> VerifyAccess(ClaimsPrincipal principal);
        Task<VerifyAccessResult> GrantAccess(string code, string redirectUrl, ClaimsPrincipal principal);
    }
}

using System.Security.Claims;
using System.Threading.Tasks;
using ABC.Leaves.Api.Models;

namespace ABC.Leaves.Api.Domain
{
    public interface IUserManager
    {
        Task<OperationResult> GetUserRefreshToken(AppUser user);
        Task<AppUserResult> CreateUserAsync(ClaimsPrincipal principal);
        Task<AppUserResult> GetUserAsync(ClaimsPrincipal principal);

        OperationResult GetEmailClaim(ClaimsPrincipal principal);
        Task<bool> HasPersistentClaim(ClaimsPrincipal principal, string claimType, string requiredValue);
    }
}

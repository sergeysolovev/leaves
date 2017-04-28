using System.Security.Claims;
using System.Threading.Tasks;
using AbcLeaves.Api.Models;
using AbcLeaves.Core;

namespace AbcLeaves.Api.Domain
{
    public interface IUserManager
    {
        Task<OperationResult> GetRefreshTokenAsync(AppUser user);
        Task<OperationResult> SaveRefreshTokenAsync(AppUser user, string token);
        Task<FindUserResult> FindUserByIdAsync(string userId);
        Task<FindUserResult> FindUserAsync(ClaimsPrincipal principal);
        Task<AddUserResult> AddUserAsync(ClaimsPrincipal principal);
        Task<UserResult> EnsureUserCreatedAsync(ClaimsPrincipal principal);
        OperationResult GetEmailClaim(ClaimsPrincipal principal);
        OperationResult GetSubjectClaim(ClaimsPrincipal principal);
        Task<bool> HasPersistentClaim(ClaimsPrincipal principal, string claimType, string requiredValue);
    }
}

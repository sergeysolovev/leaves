using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AbcLeaves.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace AbcLeaves.Api.Domain
{
    public class UserManager : IUserManager
    {
        private const string LoginProvider = "Google";
        private const string RefreshToken = "refresh_token";

        private readonly UserManager<AppUser> userManager;

        public UserManager(UserManager<AppUser> userManager)
        {
            if (userManager == null)
            {
                throw new ArgumentNullException(nameof(userManager));
            }
            if (!userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("Backing user store does not support user emails");
            }

            this.userManager = userManager;
        }

        public async Task<FindUserResult> FindUserByIdAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return FindUserResult.FailNotFoundById(userId);
            }
            return FindUserResult.Success(user);
        }

        public OperationResult GetEmailClaim(ClaimsPrincipal principal)
        {
            return GetClaim(principal, "email");
        }

        public OperationResult GetSubjectClaim(ClaimsPrincipal principal)
        {
            return GetClaim(principal, "sub");
        }

        public async Task<bool> HasPersistentClaim(ClaimsPrincipal principal,
            string claimType, string requiredValue)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }
            if (String.IsNullOrEmpty(claimType))
            {
                throw new ArgumentNullException(nameof(claimType));
            }
            var findUser = await FindUserAsync(principal);
            if (!findUser.Succeeded)
            {
                return false;
            }
            var user = findUser.User;
            var userClaims = await userManager.GetClaimsAsync(user);
            return userClaims.Any(c => c.Type == claimType && c.Value == requiredValue);
        }

        public async Task<OperationResult> SaveRefreshTokenAsync(AppUser user, string token)
        {
            var identityResult = await userManager.SetAuthenticationTokenAsync(user,
                LoginProvider, RefreshToken, token);
            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors.ToDictionary(
                    e => e.Code,
                    e => (object)e.Description);
                OperationResult.Fail(
                    "One or more errors occurred when saving a user's refresh token", errors);
            }
            return OperationResult.Success();
        }

        public async Task<OperationResult> GetRefreshTokenAsync(AppUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var refreshToken = await userManager.GetAuthenticationTokenAsync(user,
                LoginProvider, RefreshToken);
            if (String.IsNullOrEmpty(refreshToken))
            {
                return OperationResult.Fail("User's refresh token is not found");
            }
            return OperationResult.Success(refreshToken);
        }

        public async Task<FindUserResult> FindUserAsync(ClaimsPrincipal principal)
        {
            var emailResult = GetEmailClaim(principal);
            if (!emailResult.Succeeded)
            {
                return FindUserResult.FailFrom(emailResult);
            }
            var email = emailResult.Value;
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return FindUserResult.FailNotFound(email);
            }
            return FindUserResult.Success(user);
        }

        public async Task<AddUserResult> AddUserAsync(ClaimsPrincipal principal)
        {
            var findUser = await FindUserAsync(principal);
            if (findUser.Succeeded)
            {
                return AddUserResult.SuccessAlreadyExists(findUser.User);
            }
            else if (findUser.NotFound)
            {
                var email = findUser.Email;
                var user = new AppUser { UserName = email, Email = email };
                var identityResult = await userManager.CreateAsync(user);
                if (identityResult.Succeeded)
                {
                    return AddUserResult.Success(user);
                }
            }
            return AddUserResult.Fail("An error occured when adding a new user");
        }

        public async Task<UserResult> EnsureUserCreatedAsync(ClaimsPrincipal principal)
        {
            return await Operation<UserResult>
                .BeginWith(() => AddUserAsync(principal))
                .ProceedWith(addUser => UserResult.Success(addUser.User))
                .Return();
        }

        private OperationResult GetClaim(ClaimsPrincipal principal, string claimType)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }
            var claim = principal.FindFirstValue(claimType);
            if (String.IsNullOrEmpty(claim))
            {
                return OperationResult.Fail(
                    $"Failed to retrieve '{claimType}' claim from the authenticated principal"
                );
            }
            return OperationResult.Success(claim);
        }
    }
}

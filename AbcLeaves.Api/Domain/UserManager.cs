using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ABC.Leaves.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace ABC.Leaves.Api.Domain
{
    public class UserManager : IUserManager
    {
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

        public OperationResult GetEmailClaim(ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            var email = principal.FindFirstValue("email");
            if (String.IsNullOrEmpty(email))
            {
                return OperationResult.Fail(
                    "Failed to retrieve email claim from the authenticated principal"
                );
            }
            return OperationResult.Success(email);
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

            var userResult = await GetUserAsync(principal);
            if (!userResult.Succeeded)
            {
                return false;
            }
            var user = userResult.User;
            var userClaims = await userManager.GetClaimsAsync(user);
            return userClaims.Any(c => c.Type == claimType && c.Value == requiredValue);
        }

        public async Task<OperationResult> GetUserRefreshToken(AppUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var refreshToken = await userManager.GetAuthenticationTokenAsync(user,
                "Google", "refresh_token");
            if (String.IsNullOrEmpty(refreshToken))
            {
                return OperationResult.Fail("User's refresh_token is not found");
            }
            return OperationResult.Success(refreshToken);
        }

        public async Task<AppUserResult> GetUserAsync(ClaimsPrincipal principal)
        {
            var emailResult = GetEmailClaim(principal);
            if (!emailResult.Succeeded)
            {
                return AppUserResult.FailFrom(emailResult);
            }
            var email = emailResult.GetValue<string>();
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return AppUserResult.Fail($"User '{email}' not found");
            }
            return AppUserResult.Success(user);
        }

        public async Task<AppUserResult> CreateUserAsync(ClaimsPrincipal principal)
        {
            var emailResult = GetEmailClaim(principal);
            if (!emailResult.Succeeded)
            {
                return AppUserResult.FailFrom(emailResult);
            }
            var email = emailResult.GetValue<string>();
            var user = await userManager.FindByEmailAsync(email);
            if (user != null)
            {
                return AppUserResult.Fail("A user with provided email already exists");
            }
            user = new AppUser { UserName = email, Email = email };
            var identityResult = await userManager.CreateAsync(user);
            if (!identityResult.Succeeded)
            {
                return AppUserResult.Fail("An error occured when registering a new user");
            }
            return AppUserResult.Success(user);
        }
    }
}

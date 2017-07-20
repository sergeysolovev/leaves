using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AbcLeaves.Api.Models;
using AbcLeaves.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AbcLeaves.Api.Domain
{
    public class UserManager : UserManager<AppUser>
    {
        private readonly IConfiguration configuration;

        public UserManager(
            IConfiguration configuration,
            IUserStore<AppUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<AppUser> passwordHasher,
            IEnumerable<IUserValidator<AppUser>> userValidators,
            IEnumerable<IPasswordValidator<AppUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services, ILogger<UserManager<AppUser>> logger)
            : base(
                store,
                optionsAccessor,
                passwordHasher,
                userValidators,
                passwordValidators,
                keyNormalizer,
                errors,
                services,
                logger)
        {
            this.configuration = Throw.IfNull(configuration, nameof(configuration));
        }

        public string GetSubjectClaim(ClaimsPrincipal principal)
            => principal.FindFirstValue("sub");

        public async Task<IdentityResult> SetRefreshTokenAsync(AppUser user, string token)
            => await SetAuthenticationTokenAsync(user, "Google", "refresh_token", token);

        public async Task<string> GetRefreshTokenAsync(AppUser user)
            => await GetAuthenticationTokenAsync(user, "Google", "refresh_token");

        private bool IsAdmin(ClaimsPrincipal principal)
        {
            var userEmail = principal.FindFirstValue("email");
            if (String.IsNullOrEmpty(userEmail))
            {
                return false;
            }

            var admins = configuration["Admins"];
            if (String.IsNullOrEmpty(admins))
            {
                return false;
            }

            var adminEmails = admins.Split(',').Select(x => x.Trim());
            return adminEmails.Contains(userEmail);
        }

        public async Task<AppUser> GetOrCreateUserAsync(ClaimsPrincipal principal)
        {
            var user = await GetUserAsync(principal);
            if (user == null)
            {
                var userIdClaimType = Options.ClaimsIdentity.UserIdClaimType;
                var userNameClaimType = Options.ClaimsIdentity.UserNameClaimType;
                user = new AppUser {
                    Id = principal.FindFirstValue(userIdClaimType),
                    UserName = principal.FindFirstValue(userNameClaimType)
                };
                var identityResult = await CreateAsync(user);
                if (!identityResult.Succeeded)
                {
                    return null;
                }

                if (IsAdmin(principal))
                {
                    await AddClaimAsync(user, new Claim("ManageAllLeaves", "Allowed"));
                    await AddClaimAsync(user, new Claim("ApproveLeaves", "Allowed"));
                    await AddClaimAsync(user, new Claim("DeclineLeaves", "Allowed"));
                }
            }
            return user;
        }
    }
}

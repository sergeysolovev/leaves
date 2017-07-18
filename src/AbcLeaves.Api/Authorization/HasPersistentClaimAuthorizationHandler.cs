using System.Linq;
using System.Threading.Tasks;
using AbcLeaves.Api.Domain;
using Microsoft.AspNetCore.Authorization;

namespace AbcLeaves.Api
{
    public class HasPersistentClaimAuthorizationHandler : AuthorizationHandler<HasPersistentClaimRequirement>
    {
        private readonly UserManager userManager;

        public HasPersistentClaimAuthorizationHandler(UserManager userManager)
        {
            this.userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            HasPersistentClaimRequirement requirement)
        {
            var principal = context.User;
            if (principal == null)
            {
                context.Fail();
            }

            var user = await userManager.GetOrCreateUserAsync(principal);
            if (user == null)
            {
                context.Fail();
            }

            var userClaims = await userManager.GetClaimsAsync(user);
            var hasClaim = userClaims.Any(c =>
                c.Type == requirement.ClaimType &&
                c.Value == requirement.RequiredValue
            );
            if (!hasClaim)
            {
                context.Fail();
            }

            context.Succeed(requirement);
        }
    }
}

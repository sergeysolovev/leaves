using System.Linq;
using System.Threading.Tasks;
using Leaves.Api.Domain;
using Microsoft.AspNetCore.Authorization;

namespace Leaves.Api
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
                return;
            }

            var user = await userManager.GetOrCreateUserAsync(principal);
            if (user == null)
            {
                context.Fail();
                return;
            }

            var userClaims = await userManager.GetClaimsAsync(user);
            var hasClaim = userClaims.Any(c =>
                c.Type == requirement.ClaimType &&
                c.Value == requirement.RequiredValue
            );
            if (!hasClaim)
            {
                context.Fail();
                return;
            }

            context.Succeed(requirement);
        }
    }
}

using System.Threading.Tasks;
using ABC.Leaves.Api.Domain;
using ABC.Leaves.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace ABC.Leaves.Api
{
    public class HasPersistentClaimAuthorizationHandler : AuthorizationHandler<HasPersistentClaimRequirement>
    {
        private readonly IUserManager userManager;

        public HasPersistentClaimAuthorizationHandler(IUserManager appUserManager)
        {
            this.userManager = appUserManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            HasPersistentClaimRequirement requirement)
        {
            var principal = context.User;
            if (principal == null)
            {
                context.Fail();
            }
            var hasClaim = await userManager.HasPersistentClaim(principal,
                requirement.ClaimType, requirement.RequiredValue);
            if (!hasClaim)
            {
                context.Fail();
            }
            context.Succeed(requirement);
        }
    }
}

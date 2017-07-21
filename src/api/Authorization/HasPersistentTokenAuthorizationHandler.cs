using System.Threading.Tasks;
using Leaves.Api.Domain;
using Microsoft.AspNetCore.Authorization;

namespace Leaves.Api
{
    public class HasPersistentTokenAuthorizationHandler : AuthorizationHandler<HasPersistentTokenRequirement>
    {
        private readonly UserManager userManager;

        public HasPersistentTokenAuthorizationHandler(UserManager userManager)
        {
            this.userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            HasPersistentTokenRequirement requirement)
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

            var token = await userManager.GetAuthenticationTokenAsync(
                user,
                requirement.LoginProvider,
                requirement.TokenName
            );

            if (string.IsNullOrEmpty(token))
            {
                context.Fail();
            }

            context.Succeed(requirement);
        }
    }
}

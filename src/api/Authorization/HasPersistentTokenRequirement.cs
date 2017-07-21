using Microsoft.AspNetCore.Authorization;

namespace Leaves.Api
{
    public class HasPersistentTokenRequirement : IAuthorizationRequirement
    {
        public HasPersistentTokenRequirement(
            string loginProvider, string tokenName)
        {
            LoginProvider = loginProvider;
            TokenName = tokenName;
        }

        public string LoginProvider { get; private set; }
        public string TokenName { get; private set; }
    }
}

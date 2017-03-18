using Microsoft.AspNetCore.Authorization;

namespace AbcLeaves.Api
{
    public class HasPersistentClaimRequirement : IAuthorizationRequirement
    {
        public HasPersistentClaimRequirement(string claimType, string requiredValue)
        {
            ClaimType = claimType;
            RequiredValue = requiredValue;
        }

        public string ClaimType { get; private set; }
        public string RequiredValue { get; private set; }
    }
}

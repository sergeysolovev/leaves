using Microsoft.AspNetCore.Authorization;

namespace ABC.Leaves.Api
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

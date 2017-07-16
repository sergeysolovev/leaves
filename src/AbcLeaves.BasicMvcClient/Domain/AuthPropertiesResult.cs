using System.Collections.Generic;
using AbcLeaves.Core;
using Microsoft.AspNetCore.Http.Authentication;

namespace AbcLeaves.BasicMvcClient.Domain
{
    public class AuthPropertiesResult : OperationResult<AuthenticationProperties>
    {
        public AuthenticationProperties AuthProperties => base.Value;

        protected AuthPropertiesResult(AuthenticationProperties authProperties)
            : base(authProperties)
        {
        }

        protected AuthPropertiesResult(string error) : base(error)
        {
        }

        public static AuthPropertiesResult Success(AuthenticationProperties authProperties)
            => new AuthPropertiesResult(authProperties);

        public static AuthPropertiesResult Fail(string error)
            => new AuthPropertiesResult(error);
    }
}

using System.Collections.Generic;
using AbcLeaves.Core;
using Microsoft.AspNetCore.Http.Authentication;

namespace AbcLeaves.BasicMvcClient.Domain
{
    public class AuthPropertiesResult : OperationResultBase
    {
        public AuthenticationProperties AuthProperties { get; private set; }

        public AuthPropertiesResult() : base() {}
        protected AuthPropertiesResult(bool succeeded) : base(succeeded) {}
        protected AuthPropertiesResult(IOperationResult fromResult) : base(fromResult) {}

        protected AuthPropertiesResult(AuthenticationProperties authProperties)
            : base(true)
        {
            AuthProperties = authProperties;
        }

        protected AuthPropertiesResult(string error, Dictionary<string, object> details)
            : base(error, details)
        {
        }

        public static AuthPropertiesResult Success()
            => new AuthPropertiesResult(true);

        public static AuthPropertiesResult Success(AuthenticationProperties authProperties)
            => new AuthPropertiesResult(authProperties);

        public static AuthPropertiesResult Fail(string error, Dictionary<string, object> details = null)
            => new AuthPropertiesResult(error, details);

        public static AuthPropertiesResult FailFrom(IOperationResult fromResult)
            => new AuthPropertiesResult(fromResult);
    }
}

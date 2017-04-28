using System.Collections.Generic;

namespace AbcLeaves.Core
{
    public class AuthTokenResult : OperationResultBase
    {
        public string Token { get; private set; }

        public AuthTokenResult() : base() {}
        protected AuthTokenResult(bool succeeded) : base(succeeded) {}
        protected AuthTokenResult(IOperationResult fromResult) : base(fromResult) {}

        protected AuthTokenResult(string token)
            : base(true)
        {
            Token = token;
        }

        protected AuthTokenResult(string error, Dictionary<string, object> details)
            : base(error, details)
        {
        }

        public static AuthTokenResult Success(string token)
            => new AuthTokenResult(token);

        public static AuthTokenResult Fail(string error, Dictionary<string, object> details = null)
            => new AuthTokenResult(error, details);

        public static AuthTokenResult FailFrom(IOperationResult fromResult)
            => new AuthTokenResult(fromResult);
    }
}

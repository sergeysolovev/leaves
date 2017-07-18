using System;
using System.Collections.Generic;

namespace AbcLeaves.Core
{
    public class AuthTokenResult : OperationResult<String>
    {
        protected AuthTokenResult(string value) : base(value) { }
        protected AuthTokenResult(Failure failure) : base(failure) { }
        public string Token => base.Value;
        public static AuthTokenResult Succeed(string value) => new AuthTokenResult(value);
        public static AuthTokenResult Fail(string errorMessage) => Fail(new Failure(errorMessage));
        public static AuthTokenResult Fail(Failure failure) => new AuthTokenResult(failure);
    }
}

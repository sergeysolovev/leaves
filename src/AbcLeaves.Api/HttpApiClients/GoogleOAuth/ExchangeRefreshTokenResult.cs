using System;
using System.Collections.Generic;
using AbcLeaves.Api.Operations;
using AbcLeaves.Utils;

namespace AbcLeaves.Api
{
    public class ExchangeRefreshTokenResult : OperationResult
    {
        public string AccessToken { get; private set; }

        public ExchangeRefreshTokenResult() : base() { }

        protected ExchangeRefreshTokenResult(string accessToken) : base()
        {
            AccessToken = Throw.IfNullOrEmpty(accessToken, nameof(accessToken));
        }

        protected ExchangeRefreshTokenResult(Failure failure) : base(failure) { }

        public static ExchangeRefreshTokenResult Success(string accessToken)
            => new ExchangeRefreshTokenResult(accessToken);

        public static ExchangeRefreshTokenResult Fail(string error)
            => new ExchangeRefreshTokenResult(new Failure(error));
    }
}

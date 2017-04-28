using System;
using System.Collections.Generic;
using AbcLeaves.Core;

namespace AbcLeaves.Api
{
    public class ExchangeRefreshTokenResult : OperationResultBase
    {
        public string AccessToken { get; private set; }

        public ExchangeRefreshTokenResult() : base() { }

        protected ExchangeRefreshTokenResult(string accessToken)
            : base(true)
        {
            if (String.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            AccessToken = accessToken;
        }

        protected ExchangeRefreshTokenResult(string error, Dictionary<string, object> details)
            : base(error, details)
        {
        }

        protected ExchangeRefreshTokenResult(IOperationResult result)
            : base(result)
        {
        }

        public static ExchangeRefreshTokenResult Success(string accessToken)
            => new ExchangeRefreshTokenResult(accessToken);

        public static ExchangeRefreshTokenResult FailFrom(IOperationResult result)
            => new ExchangeRefreshTokenResult(result);

        public static ExchangeRefreshTokenResult Fail(string error,
            Dictionary<string, object> details = null)
        {
            return new ExchangeRefreshTokenResult(error, details);
        }
    }
}

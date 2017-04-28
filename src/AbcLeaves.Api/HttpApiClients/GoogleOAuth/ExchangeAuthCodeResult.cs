using System.Collections.Generic;
using AbcLeaves.Core;

namespace AbcLeaves.Api
{
    public class ExchangeAuthCodeResult : OperationResultBase
    {
        public struct OAuthCodeFlowTokens
        {
            public string IdToken { get; set; }
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
        }

        public OAuthCodeFlowTokens Tokens { get; private set; }

        public ExchangeAuthCodeResult() : base() { }

        protected ExchangeAuthCodeResult(string idToken, string accessToken, string refreshToken)
            : base(true)
        {
            Tokens = new OAuthCodeFlowTokens
            {
                IdToken = idToken,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        protected ExchangeAuthCodeResult(string error, Dictionary<string, object> details)
            : base(error, details)
        {
        }

        protected ExchangeAuthCodeResult(IOperationResult result)
            : base(result)
        {
        }

        public static ExchangeAuthCodeResult Success(string idToken, string accessToken, string refreshToken)
            => new ExchangeAuthCodeResult(idToken, accessToken, refreshToken);

        public static ExchangeAuthCodeResult FailFrom(IOperationResult result)
            => new ExchangeAuthCodeResult(result);

        public static ExchangeAuthCodeResult Fail(string error,
            Dictionary<string, object> details = null)
        {
            return new ExchangeAuthCodeResult(error, details);
        }
    }
}

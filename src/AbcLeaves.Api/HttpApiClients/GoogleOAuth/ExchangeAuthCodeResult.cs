using System.Collections.Generic;
using AbcLeaves.Core;

namespace AbcLeaves.Api
{
    public class ExchangeAuthCodeResult : OperationResult
    {
        public struct OAuthCodeFlowTokens
        {
            public string IdToken { get; set; }
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
        }

        public OAuthCodeFlowTokens Tokens { get; private set; }

        protected ExchangeAuthCodeResult(string idToken, string accessToken, string refreshToken)
            : base()
        {
            Tokens = new OAuthCodeFlowTokens
            {
                IdToken = idToken,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        protected ExchangeAuthCodeResult(string error) : base(error) { }

        public static ExchangeAuthCodeResult Success(string idToken, string accessToken, string refreshToken)
            => new ExchangeAuthCodeResult(idToken, accessToken, refreshToken);

        public static ExchangeAuthCodeResult Fail(string error)
            => new ExchangeAuthCodeResult(error);
    }
}

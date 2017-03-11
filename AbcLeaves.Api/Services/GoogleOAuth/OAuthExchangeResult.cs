using Newtonsoft.Json.Linq;

namespace ABC.Leaves.Api.Services
{
    public class OAuthExchangeResult : IOperationResult
    {
        public static OAuthExchangeResult Success(JObject response)
        {
            return new OAuthExchangeResult
            {
                Response = response,
                IdToken = response.Value<string>("id_token"),
                AccessToken = response.Value<string>("access_token"),
                TokenType = response.Value<string>("token_type"),
                RefreshToken = response.Value<string>("refresh_token"),
                ExpiresIn = response.Value<string>("expires_in"),
                Succeeded = true
            };
        }

        public static OAuthExchangeResult Fail(string message)
        {
            return new OAuthExchangeResult { ErrorMessage = message };
        }

        public bool Succeeded { get; set; }
        public JObject Response { get; set; }
        public string IdToken { get; set; }
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public string RefreshToken { get; set; }
        public string ExpiresIn { get; set; }
        public string ErrorMessage { get; set; }
    }
}

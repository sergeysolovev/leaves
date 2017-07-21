using Newtonsoft.Json.Linq;

namespace Leaves.Api.Services
{
    public class OAuthExchangeResponse
    {
        public OAuthExchangeResponse(JObject response)
        {
            Response = response;
            IdToken = response.Value<string>("id_token");
            AccessToken = response.Value<string>("access_token");
            TokenType = response.Value<string>("token_type");
            RefreshToken = response.Value<string>("refresh_token");
            ExpiresIn = response.Value<string>("expires_in");
        }

        public JObject Response { get; set; }
        public string IdToken { get; set; }
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public string RefreshToken { get; set; }
        public string ExpiresIn { get; set; }
    }
}

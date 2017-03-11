using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace ABC.Leaves.Api.Services
{
    public class GoogleOAuthService : IGoogleOAuthService
    {
        private readonly GoogleOAuthOptions authOptions;
        private readonly HttpClient backchannel;

        public GoogleOAuthService(IOptions<GoogleOAuthOptions> authOptionsAccessor,
            HttpClientHandler httpBackchannelHandler)
        {
            this.authOptions = authOptionsAccessor.Value;
            this.backchannel = new HttpClient(httpBackchannelHandler);
        }

        public string BuildOfflineAccessChallengeUrl(string redirectUrl, string state)
        {
            return QueryHelpers.AddQueryString(
                uri: authOptions.AuthUri,
                queryString: new Dictionary<string, string> {
                    ["response_type"] = "code",
                    ["client_id"] = authOptions.ClientId,
                    ["scope"] = String.Join(" ", authOptions.Scopes),
                    ["redirect_uri"] = redirectUrl,
                    ["access_type"] = "offline",
                    ["prompt"] = "consent",
                    ["include_granted_scopes"] = "true",
                    ["state"] = state
                });
        }

        public async Task<OAuthExchangeResult> ExchangeCode(string code, string redirectUrl)
        {
            var tokenRequestContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = authOptions.ClientId,
                ["client_secret"] = authOptions.ClientSecret,
                ["redirect_uri"] = redirectUrl,
                ["grant_type"] = "authorization_code"
            });
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, authOptions.RefreshTokenUri);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requestMessage.Content = tokenRequestContent;
            using (var response = await backchannel.SendAsync(requestMessage))
            {
                if (response.IsSuccessStatusCode)
                {
                    var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
                    return OAuthExchangeResult.Success(payload);
                }
                else
                {
                    return OAuthExchangeResult.Fail("OAuth token endpoint failure: " +
                        await DisplayHttpResponse(response)
                    );
                }
            }
        }

        public async Task<OAuthExchangeResult> ExchangeRefreshToken(string refreshToken)
        {
            var tokenRequestContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["refresh_token"] = refreshToken,
                ["client_id"] = authOptions.ClientId,
                ["client_secret"] = authOptions.ClientSecret,
                ["grant_type"] = "refresh_token"
            });
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, authOptions.TokenUri);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requestMessage.Content = tokenRequestContent;
            using (var response = await backchannel.SendAsync(requestMessage))
            {
                if (response.IsSuccessStatusCode)
                {
                    var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
                    return OAuthExchangeResult.Success(payload);
                }
                else
                {
                    return OAuthExchangeResult.Fail("OAuth token endpoint failure: " +
                        await DisplayHttpResponse(response)
                    );
                }
            }
        }

        private static async Task<string> DisplayHttpResponse(HttpResponseMessage response)
        {
            var output = new System.Text.StringBuilder();
            output.Append("Status: " + response.StatusCode + ";");
            output.Append("Headers: " + response.Headers.ToString() + ";");
            output.Append("Body: " + await response.Content.ReadAsStringAsync() + ";");
            return output.ToString();
        }
    }
}

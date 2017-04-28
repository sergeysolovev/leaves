using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AbcLeaves.Api.Helpers;
using AbcLeaves.Core;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace AbcLeaves.Api.Services
{
    public class GoogleOAuthService : IGoogleOAuthService
    {
        private readonly GoogleOAuthOptions authOptions;
        private readonly HttpClient backchannel;
        private readonly IBackChannelHelper backchannelHelper;

        public GoogleOAuthService(
            IOptions<GoogleOAuthOptions> authOptionsAccessor,
            HttpMessageHandler httpBackchannelHandler,
            IBackChannelHelper backchannelHelper)
        {
            this.authOptions = authOptionsAccessor.Value;
            this.backchannel = new HttpClient(httpBackchannelHandler);
            this.backchannelHelper = backchannelHelper;
        }

        public async Task<ExchangeAuthCodeResult> ExchangeAuthCode(string code, string redirectUrl)
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
                var details = new Dictionary<string, object>();
                var error = String.Empty;
                if (response.IsSuccessStatusCode)
                {
                    var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
                    var exchangeResponse = new OAuthExchangeResponse(payload);
                    var idToken = exchangeResponse.IdToken;
                    var accessToken = exchangeResponse.AccessToken;
                    var refreshToken = exchangeResponse.RefreshToken;
                    if (String.IsNullOrEmpty(idToken))
                    {
                        details.Add("id_token", "Failed to retrieve oauth id_token");
                    }
                    if (String.IsNullOrEmpty(accessToken))
                    {
                        details.Add("access_token", "Failed to retrieve oauth access_token");
                    }
                    if (String.IsNullOrEmpty(refreshToken))
                    {
                        details.Add("refresh_token", "Failed to retrieve oauth refresh_token");
                    }
                    if (details.Any())
                    {
                        error = "Failed to exchange oauth authorization code with tokens";
                        return ExchangeAuthCodeResult.Fail(error, details);
                    }
                    return ExchangeAuthCodeResult.Success(idToken, accessToken, refreshToken);
                }
                else
                {
                    error = "OAuth token endpoint failure, see details from more info";
                    details = await backchannelHelper.GetResponseDetailsAsync(response);
                    return ExchangeAuthCodeResult.Fail(error, details);
                }
            }
        }

        public async Task<ExchangeRefreshTokenResult> ExchangeRefreshToken(string refreshToken)
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
                    var exchangeResponse = new OAuthExchangeResponse(payload);
                    var accessToken = exchangeResponse.AccessToken;
                    if (String.IsNullOrEmpty(accessToken))
                    {
                        return ExchangeRefreshTokenResult.Fail(
                            "Failed to exchange the refresh token with an access token");
                    }
                    return ExchangeRefreshTokenResult.Success(accessToken);
                }
                else
                {
                    var error = "OAuth token endpoint failure, see details from more info";
                    var details = await backchannelHelper.GetResponseDetailsAsync(response);
                    return ExchangeRefreshTokenResult.Fail(error, details);
                }
            }
        }

        public async Task<VerifyAccessResult> ValidateRefreshTokenAsync(string refreshToken)
        {
            // todo: implement validation
            return await Task.FromResult(VerifyAccessResult.Success);
        }
    }
}

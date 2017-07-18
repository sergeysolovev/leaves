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
    public class GoogleOAuthClient
    {
        private readonly GoogleOAuthOptions options;
        private readonly IBackchannel backchannel;

        public GoogleOAuthClient(
            IOptions<GoogleOAuthOptions> options,
            IBackchannelFactory backchannelFactory)
        {
            var factory = Throw.IfNull(backchannelFactory, nameof(backchannelFactory));
            this.options = Throw.IfNull(options, nameof(options)).Value;
            this.backchannel = factory.Create();
        }

        public async Task<ExchangeAuthCodeResult> ExchangeAuthCode(string code, string redirectUrl)
        {
            var error = "Failed to exchange oauth authorization code with tokens";

            var tokenRequestContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = options.ClientId,
                ["client_secret"] = options.ClientSecret,
                ["redirect_uri"] = redirectUrl,
                ["grant_type"] = "authorization_code"
            });

            var url = options.RefreshTokenUri;
            var exchangeResult = await backchannel.PostAsync(url, x => x
                .WithContent(tokenRequestContent)
                .AcceptJson()
            );

            if (!exchangeResult.Succeeded)
            {
                ExchangeAuthCodeResult.Fail(error);
            }

            var response = exchangeResult.Response;

            if (!response.IsSuccessStatusCode)
            {
                ExchangeAuthCodeResult.Fail(error);
            }

            var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
            var exchangeResponse = new OAuthExchangeResponse(payload);
            var idToken = exchangeResponse.IdToken;
            var accessToken = exchangeResponse.AccessToken;
            var refreshToken = exchangeResponse.RefreshToken;
            var missingTokens = new List<string>();

            if (String.IsNullOrEmpty(idToken))
            {
                missingTokens.Add("id_token");
            }
            if (String.IsNullOrEmpty(accessToken))
            {
                missingTokens.Add("access_token");
            }
            if (String.IsNullOrEmpty(refreshToken))
            {
                missingTokens.Add("refresh_token");
            }

            if (missingTokens.Any())
            {
                return ExchangeAuthCodeResult.Fail(
                    $"Failed to retrieve oauth tokens: {String.Join(",", missingTokens)}"
                );
            }

            return ExchangeAuthCodeResult.Success(idToken, accessToken, refreshToken);
        }

        public async Task<ExchangeRefreshTokenResult> ExchangeRefreshToken(string refreshToken)
        {
            var error = "Failed to exchange the refresh token with an access token";

            var tokenRequestContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["refresh_token"] = refreshToken,
                ["client_id"] = options.ClientId,
                ["client_secret"] = options.ClientSecret,
                ["grant_type"] = "refresh_token"
            });

            var url = options.TokenUri;
            var exchangeResult = await backchannel.PostAsync(url, x => x
                .WithContent(tokenRequestContent)
                .AcceptJson()
            );

            if (!exchangeResult.Succeeded)
            {
                return ExchangeRefreshTokenResult.Fail(error);
            }

            var response = exchangeResult.Response;

            if (!response.IsSuccessStatusCode)
            {
                return ExchangeRefreshTokenResult.Fail(error);
            }

            var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
            var exchangeResponse = new OAuthExchangeResponse(payload);
            var accessToken = exchangeResponse.AccessToken;

            if (String.IsNullOrEmpty(accessToken))
            {
                return ExchangeRefreshTokenResult.Fail(
                    "Failed to exchange the refresh_token with an access_token"
                );
            }

            return ExchangeRefreshTokenResult.Success(accessToken);
        }
    }
}

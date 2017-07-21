using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Leaves.Utils;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Leaves.Api.Services
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

        public async Task<OAuthExchangeResponse> ExchangeAuthCode(string code, string redirectUrl)
        {
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
                return null;
            }

            var response = exchangeResult.Response;
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            try
            {
                var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
                return new OAuthExchangeResponse(payload);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        public async Task<string> ExchangeRefreshToken(string refreshToken)
        {
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
                return null;
            }

            var response = exchangeResult.Response;
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            try
            {
                var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
                var exchangeResponse = new OAuthExchangeResponse(payload);
                return exchangeResponse.AccessToken;
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}

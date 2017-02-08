using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ABC.Leaves.Api.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly GoogleAuthOptions authOptions;
        private readonly IHttpClient httpClient;

        public GoogleAuthService(IOptions<GoogleAuthOptions> authOptionsAccessor,
            IHttpClient httpClient)
        {
            this.authOptions = authOptionsAccessor.Value;
            this.httpClient = httpClient;
        }

        public string GetAuthUrl(string redirectUrl)
        {
            return QueryHelpers.AddQueryString(
                uri: authOptions.AuthUri,
                queryString: new Dictionary<string, string> {
                    ["response_type"] = authOptions.ResponseType,
                    ["client_id"] = authOptions.ClientId,
                    ["scope"] = String.Join(" ", authOptions.Scopes),
                    ["redirect_uri"] = redirectUrl
                });
        }

        public string GetAccessToken(string code, string redirectUrl)
        {
            var values = new Dictionary<string, string>
            {
                {"code", code},
                {"client_id", authOptions.ClientId},
                {"client_secret", authOptions.ClientSecret},
                {"redirect_uri", redirectUrl},
                {"grant_type", "authorization_code"}
            };
            var content = new FormUrlEncodedContent(values);
            var response = httpClient.PostAsync(authOptions.TokenUri, content).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            var reponseValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);
            var token = reponseValues["access_token"];
            return token;
        }
    }
}

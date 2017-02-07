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

        public GoogleAuthService(IOptions<GoogleAuthOptions> authOptionsAccessor)
        {
            authOptions = authOptionsAccessor.Value;
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
            using (var client = new HttpClient())
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
                var response = client.PostAsync("https://www.googleapis.com/oauth2/v4/token", content).Result;
                var responseString = response.Content.ReadAsStringAsync().Result;
                var reponseValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);
                var token = reponseValues["access_token"];
                return token;
            }
        }
    }
}

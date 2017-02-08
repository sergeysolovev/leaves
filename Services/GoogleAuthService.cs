using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<ObjectResult> GetAccessTokenAsync(string code, string redirectUrl)
        {
            var httpContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = authOptions.ClientId,
                ["client_secret"] = authOptions.ClientSecret,
                ["redirect_uri"] = redirectUrl,
                ["grant_type"] = "authorization_code"
            });
            using (var response = await httpClient.PostAsync(authOptions.TokenUri, httpContent))
            {
                var result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    string accessToken;
                    var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
                    if (jsonResult.TryGetValue("access_token", out accessToken))
                    {
                        return new OkObjectResult(accessToken);
                    }
                }
                var error = new ErrorDetails
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    DeveloperMessage =
                        "Failed to exchange an authorization code for an access token. " +
                        $"Google responsed '{result}' with status code '{(int)response.StatusCode}'"
                };
                return new ObjectResult(error) { StatusCode = error.StatusCode };
            }
        }
    }
}

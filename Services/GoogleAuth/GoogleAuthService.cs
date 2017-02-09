using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ABC.Leaves.Api.GoogleAuth.Dto;
using ABC.Leaves.Api.Services;
using ABC.Leaves.Api.Services.Dto;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ABC.Leaves.Api.GoogleAuth
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

        public GetAuthUrlOutput GetAuthUrl(GetAuthUrlInput input)
        {
            return new GetAuthUrlOutput
            {
                AuthUrl = QueryHelpers.AddQueryString(
                    uri: authOptions.AuthUri,
                    queryString: new Dictionary<string, string> {
                        ["response_type"] = authOptions.ResponseType,
                        ["client_id"] = authOptions.ClientId,
                        ["scope"] = String.Join(" ", authOptions.Scopes),
                        ["redirect_uri"] = input.RedirectUrl
                    })
            };
        }

        public async Task<GetAccessTokenAsyncOutput> GetAccessTokenAsync(GetAccessTokenAsyncInput input)
        {
            var httpContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["code"] = input.Code,
                ["client_id"] = authOptions.ClientId,
                ["client_secret"] = authOptions.ClientSecret,
                ["redirect_uri"] = input.RedirectUrl,
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
                        return new GetAccessTokenAsyncOutput { AccessToken = accessToken };
                    }
                }
                var error = new ErrorDto
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    DeveloperMessage =
                        "Failed to exchange an authorization code for an access token. " +
                        $"Google responsed '{result}' with status code '{(int)response.StatusCode}'"
                };
                return new GetAccessTokenAsyncOutput { Error = error };
            }
        }
    }
}

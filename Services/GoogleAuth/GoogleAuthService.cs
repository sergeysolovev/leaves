using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ABC.Leaves.Api.GoogleAuth.Dto;
using ABC.Leaves.Api.Services;
using ABC.Leaves.Api.Services.Dto;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public GetAuthUrlResult GetAuthUrl(string redirectUrl)
        {
            return new GetAuthUrlResult
            {
                AuthUrl = QueryHelpers.AddQueryString(
                    uri: authOptions.AuthUri,
                    queryString: new Dictionary<string, string> {
                        ["response_type"] = authOptions.ResponseType,
                        ["client_id"] = authOptions.ClientId,
                        ["scope"] = String.Join(" ", authOptions.Scopes),
                        ["redirect_uri"] = redirectUrl
                    })
            };
        }

        public async Task<GetAccessTokenResult> GetAccessTokenAsync(string code, string redirectUrl)
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
                if (!response.IsSuccessStatusCode)
                {
                    return new GetAccessTokenResult
                    {
                        Error = new ErrorDto
                        {
                            DeveloperMessage =
                                "Failed to exchange an authorization code for an access token. " +
                                $"Google responsed '{result}' with status code '{(int)response.StatusCode}'"
                        }
                    };
                }
                string accessToken = RetrieveAccessToken(result);
                if (accessToken == null)
                {
                    return new GetAccessTokenResult
                    {
                        Error = new ErrorDto("An error occured when retrieving access token")
                    };
                }
                return new GetAccessTokenResult { AccessToken = accessToken };
            }
        }

        public async Task<GetAccessTokenInfoResult> GetAccessTokenInfoAsync(string accessToken)
        {
            string tokenInfoUrl = QueryHelpers.AddQueryString(
                uri: authOptions.TokenInfoUri,
                name: "access_token",
                value: accessToken);
            using (var response = await httpClient.GetAsync(tokenInfoUrl))
            {
                var result = await response.Content.ReadAsStringAsync();
                ErrorDto error;
                if (response.IsSuccessStatusCode)
                {
                    string email;
                    var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
                    if (!jsonResult.TryGetValue("email", out email))
                    {
                        error = new ErrorDto
                        {
                            DeveloperMessage = "Failed to retrieve 'email' value from access token."
                        };
                        return new GetAccessTokenInfoResult { Error = error };
                    }
                    return new GetAccessTokenInfoResult { Email = email };
                }
                error = new ErrorDto
                {
                    DeveloperMessage =
                        "An error occured when retrieving access token info. " +
                        $"Google responsed '{result}' with status code '{(int)response.StatusCode}'"
                };
                return new GetAccessTokenInfoResult { Error = error };
            }
        }

        public async Task<ValidateAccessTokenResult> ValidateAccessTokenAsync(string accessToken)
        {
            var getTokenResult = await GetAccessTokenInfoAsync(accessToken);
            if (getTokenResult.Error != null)
            {
                return new ValidateAccessTokenResult {
                    IsValid = false,
                    Error = new ErrorDto {
                        DeveloperMessage = "Google access token is not valid. " +
                            getTokenResult.Error.DeveloperMessage
                    }
                };
            }
            return new ValidateAccessTokenResult { IsValid = true };
        }

        private static string RetrieveAccessToken(string tokenResponse)
        {
            try
            {
                var json = JObject.Parse(tokenResponse);
                return (string)json["access_token"];
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}

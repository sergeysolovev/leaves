using System.Collections.Generic;
using System.Net.Http;
using Google.Apis.Calendar.v3;
using Google.Apis.Gmail.v1;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ABC.Leaves.Api.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly string[] scopes =
        {
            CalendarService.Scope.Calendar,
            GmailService.Scope.GmailReadonly,
            "profile",
        };

        private readonly GoogleAuthOptions authOptions;

        public GoogleAuthService(IOptions<GoogleAuthOptions> authOptionsAccessor)
        {
            authOptions = authOptionsAccessor.Value;
        }

        public string GetAuthUrl(string redirectUrl)
        {
            string baseAddress = authOptions.AuthUri;

            var scope = string.Join("%20", scopes);

            string urlParameters = $"?scope={scope}&redirect_uri={redirectUrl}&response_type=code&client_id={authOptions.ClientId}";

            return baseAddress + urlParameters;
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

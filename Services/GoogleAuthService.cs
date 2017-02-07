using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Gmail.v1;
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

        private readonly ClientSecrets clientSecrets;

        public GoogleAuthService()
        {
            using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                clientSecrets = GoogleClientSecrets.Load(stream).Secrets;
            }
        }

        public string GetAuthUrl(string redirectUrl)
        {
            const string baseAddress = "https://accounts.google.com/o/oauth2/v2/auth";

            var scope = string.Join("%20", scopes);

            string urlParameters = $"?scope={scope}&redirect_uri={redirectUrl}&response_type=code&client_id={clientSecrets.ClientId}";

            return baseAddress + urlParameters;
        }

        public string GetAccessToken(string code, string redirectUrl)
        {
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    {"code", code},
                    {"client_id", clientSecrets.ClientId},
                    {"client_secret", clientSecrets.ClientSecret},
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

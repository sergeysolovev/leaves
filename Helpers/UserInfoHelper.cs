using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace ABC.Leaves.Api.Helpers
{
    public static class UserInfoHelper
    {
        public static string GetUserGmailAddress(string token)
        {
            using (var client = new HttpClient())
            {
                var userInfo = client.GetStringAsync("https://www.googleapis.com/oauth2/v1/userinfo?alt=json&access_token=" + token).Result;
                var userInfoValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(userInfo);
                var userId = userInfoValues["id"];
                var userProfile = client.GetStringAsync("https://www.googleapis.com/gmail/v1/users/" + userId + "/profile?alt=json&access_token=" + token).Result;
                var userProfileValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(userProfile);
                var gmailLogin = userProfileValues["emailAddress"];
                return gmailLogin;
            }
        }
    }
}

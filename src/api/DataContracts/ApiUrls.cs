using Newtonsoft.Json;

namespace Leaves.Api.DataContract
{
    public class ApiUrlsContract
    {
        [JsonProperty("calendar_auth_url")]
        public string GoogleCalendarAuthorizationsUrl { get; set; }

        [JsonProperty("leaves_url")]
        public string LeavesUrl { get; set; }
    }
}

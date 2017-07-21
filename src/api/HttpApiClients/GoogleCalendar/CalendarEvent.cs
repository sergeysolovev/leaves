using Newtonsoft.Json;

namespace Leaves.Api.Services
{
    public class CalendarEvent
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("end")]
        public CalendarEventDateTime End { get; set; }

        [JsonProperty("start")]
        public CalendarEventDateTime Start { get; set; }
    }
}

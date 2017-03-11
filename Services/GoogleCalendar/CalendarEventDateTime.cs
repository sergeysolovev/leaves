using System;
using Newtonsoft.Json;

namespace ABC.Leaves.Api.Services
{
    public class CalendarEventDateTime
    {
        [JsonProperty("dateTime")]
        public DateTime DateTime { get; set; }

        [JsonProperty("timeZone")]
        public string TimeZone { get; set; }
    }
}

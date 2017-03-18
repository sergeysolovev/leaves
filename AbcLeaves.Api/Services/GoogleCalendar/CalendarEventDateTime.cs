using System;
using Newtonsoft.Json;

namespace AbcLeaves.Api.Services
{
    public class CalendarEventDateTime
    {
        public CalendarEventDateTime(DateTime dateTime)
        {
            DateTime = dateTime;
        }

        [JsonProperty("dateTime")]
        public DateTime DateTime { get; set; }

        [JsonProperty("timeZone")]
        public string TimeZone { get; set; }
    }
}

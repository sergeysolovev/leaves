using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace ABC.Leaves.Api.Services
{
    public class GoogleCalendarService : IGoogleCalendarService
    {
        private readonly GoogleCalendarOptions calendarOptions;
        private readonly HttpClient backchannel;

        public GoogleCalendarService(IOptions<GoogleCalendarOptions> calendarOptionsAccessor,
            HttpClientHandler httpBackchannelHandler)
        {
            this.calendarOptions = calendarOptionsAccessor.Value;
            this.backchannel = new HttpClient(httpBackchannelHandler);
        }

        public async Task<AddEventResult> AddEventAsync(string accessToken, DateTime start, DateTime end)
        {
            var calendarEvent = CreateEvent(start, end);
            var calendarEventJson = JsonConvert.SerializeObject(calendarEvent,
                new JsonSerializerSettings
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                });
            var eventsUri = calendarOptions.EventsUri;
            var request = new HttpRequestMessage(HttpMethod.Post, eventsUri);
            request.Content = new StringContent(calendarEventJson, Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            using (var response = await backchannel.SendAsync(request))
            {
                var result = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return AddEventResult.Fail(
                        "An error occured when adding event to the Google calendar. " +
                        $"Google responsed '{result}' with status code '{(int)response.StatusCode}'"
                    );
                }
                string eventUri = GetEventUriFromJson(result);
                if (String.IsNullOrEmpty(eventUri))
                {
                    // todo: log error
                }
                return AddEventResult.Success(eventUri);
            }
        }

        private CalendarEvent CreateEvent(DateTime start, DateTime end)
        {
            return new CalendarEvent
            {
                Start = new CalendarEventDateTime { DateTime = start },
                End = new CalendarEventDateTime { DateTime = end },
                Summary = calendarOptions.LeaveEventSummary,
                Description = calendarOptions.LeaveEventDescription
            };
        }

        private string GetEventUriFromJson(string json)
        {
            try
            {
                var jsonObject = JObject.Parse(json);
                return jsonObject.Value<string>("htmlLink");
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}

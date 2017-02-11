using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ABC.Leaves.Api.GoogleCalendar;
using ABC.Leaves.Api.GoogleCalendar.Dto;
using ABC.Leaves.Api.Services;
using ABC.Leaves.Api.Services.Dto;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ABC.Leaves.Api.GoogleAuth
{
    public class GoogleCalendarService : IGoogleCalendarService
    {
        private readonly GoogleCalendarOptions calendarOptions;
        private readonly IHttpClient httpClient;

        public GoogleCalendarService(IOptions<GoogleCalendarOptions> calendarOptionsAccessor,
            IHttpClient httpClient)
        {
            this.calendarOptions = calendarOptionsAccessor.Value;
            this.httpClient = httpClient;
        }

        public async Task<AddEventResult> AddEventAsync(string accessToken, DateTime start, DateTime end)
        {
            var calendarEvent = CreateEvent(start, end);
            var calendarEventJson = JsonConvert.SerializeObject(calendarEvent,
                new JsonSerializerSettings
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                });
            var httpContent = new StringContent(calendarEventJson, Encoding.UTF8, "application/json");
            var eventsUri = calendarOptions.EventsUri;
            using (var response = await httpClient.PostAsync(eventsUri, httpContent, accessToken))
            {
                var result = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return new AddEventResult
                    {
                        Error = new ErrorDto {
                            DeveloperMessage = "An error occured when adding event to the Google calendar. " +
                                $"Google responsed '{result}' with status code '{(int)response.StatusCode}'"
                        }
                    };
                }
                string eventUri = JsonHelper.GetPropertyValue(result, "htmlLink");
                if (String.IsNullOrEmpty(eventUri))
                {
                    return new AddEventResult
                    {
                        EventAdded = true,
                        Error = new ErrorDto("Failed to retrieve event uri from created event resource")
                    };
                }
                return new AddEventResult { EventAdded = true, EventUri = eventUri };
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
    }
}

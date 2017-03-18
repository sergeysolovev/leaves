using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using AutoMapper;
using AbcLeaves.Api.Helpers;

namespace AbcLeaves.Api.Services
{
    public class GoogleCalendarService : IGoogleCalendarService
    {
        private const string EventsUri = "https://www.googleapis.com/calendar/v3/calendars/primary/events";

        private readonly IMapper mapper;
        private readonly HttpClient backchannel;
        private readonly IBackChannelHelper backchannelHelper;

        public GoogleCalendarService(
            IMapper mapper,
            IBackChannelHelper backchannelHelper,
            HttpClientHandler httpBackchannelHandler)
        {
            this.mapper = mapper;
            this.backchannelHelper = backchannelHelper;
            this.backchannel = new HttpClient(httpBackchannelHandler);
        }

        public async Task<OperationResult> AddEventAsync(CalendarEventAddDto eventDto)
        {
            var accessToken = eventDto.AccessToken;
            var calendarEvent = mapper.Map<CalendarEventAddDto, CalendarEvent>(eventDto);
            var calendarEventJson = JsonConvert.SerializeObject(calendarEvent,
                new JsonSerializerSettings
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                });
            var request = new HttpRequestMessage(HttpMethod.Post, EventsUri);
            request.Content = new StringContent(calendarEventJson, Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            using (var response = await backchannel.SendAsync(request))
            {
                if (!response.IsSuccessStatusCode)
                {
                    var error = "An error occured when adding event to the Google calendar";
                    var details = await backchannelHelper.GetResponseDetailsAsync(response);
                    return OperationResult.Fail(error, details);
                }
                var responseContent = await response.Content.ReadAsStringAsync();
                var eventUri = GetEventUriFromJson(responseContent);
                if (String.IsNullOrEmpty(eventUri))
                {
                        // todo: log error
                }
                return OperationResult.Success(eventUri);
            }
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

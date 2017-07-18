using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AutoMapper;
using AbcLeaves.Core;
using Microsoft.Extensions.Options;
using System.Net.Http;

namespace AbcLeaves.Api.Services
{
    public class GoogleCalendarClient
    {
        private readonly IMapper mapper;
        private readonly IBackchannel backchannel;

        public GoogleCalendarClient(
            IOptions<GoogleCalendarOptions> options,
            IBackchannelFactory backchannelFactory,
            IMapper mapper)
        {
            var factory = Throw.IfNull(backchannelFactory, nameof(backchannelFactory));
            var baseUrl = Throw.IfNull(options, nameof(options)).Value.BaseUrl;
            this.mapper = Throw.IfNull(mapper, nameof(mapper));
            this.backchannel = factory.Create(baseUrl);
        }

        public async Task<StringResult> AddEventAsync(AddCalendarEventContract eventContract)
        {
            var error = "Failed to add an event to Google Calendar";
            var calendarEvent = mapper.Map<AddCalendarEventContract, CalendarEvent>(eventContract);

            var json = JsonConvert.SerializeObject(calendarEvent, new JsonSerializerSettings {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            });

            var result = await backchannel.PostAsync("calendars/primary/events", x => x
                .WithBearerToken(eventContract.AccessToken)
                .WithJsonContent(json)
            );

            if (!result.Succeeded)
            {
                return StringResult.Fail(error);
            }

            var response = result.Response;

            if (!response.IsSuccessStatusCode)
            {
                return StringResult.Fail(error);
            }

            return GetEventUriFromJson(
                await response.Content.ReadAsStringAsync()
            );
        }

        private StringResult GetEventUriFromJson(string json)
        {
            try
            {
                var jsonObject = JObject.Parse(json);
                var eventUri = jsonObject.Value<string>("htmlLink");
                return StringResult.Succeed(eventUri);
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}

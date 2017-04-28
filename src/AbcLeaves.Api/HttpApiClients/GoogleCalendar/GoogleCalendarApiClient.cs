using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AutoMapper;
using AbcLeaves.Core;
using Microsoft.Extensions.Options;

namespace AbcLeaves.Api.Services
{
    public class GoogleCalendarApiClient
    {
        private readonly IMapper mapper;
        private readonly IHttpApiClientService clientService;

        public GoogleCalendarApiClient(
            IOptions<HttpApiClientOptions<GoogleCalendarApiClient>> options,
            IHttpApiClientServiceFactory clientFactory,
            IMapper mapper)
        {

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (options.Value == null)
            {
                throw new ArgumentNullException(nameof(options.Value));
            }
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            this.clientService = clientFactory.Create(options.Value);
            this.mapper = mapper;
        }

        public async Task<OperationResult> AddEventAsync(CalendarEventAddDto eventDto)
        {
            var calendarEvent = mapper.Map<CalendarEventAddDto, CalendarEvent>(eventDto);
            var json = JsonConvert.SerializeObject(calendarEvent, new JsonSerializerSettings {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            });
            return await OperationFlow<OperationResult>
                .BeginWith(() => clientService
                    .PostAsync("calendars/primary/events", x => x
                        .AddBearerToken(eventDto.AccessToken)
                        .UseRequestContent(() => new JsonContent(json))))
                .ProceedWith(callApi => GetEventUriFromJson(callApi.ApiResponseDetails.Body))
                .Return();
        }

        private OperationResult GetEventUriFromJson(string json)
        {
            try
            {
                var jsonObject = JObject.Parse(json);
                var eventUri = jsonObject.Value<string>("htmlLink");
                return OperationResult.Success(eventUri);
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}

﻿using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AutoMapper;
using Microsoft.Extensions.Options;
using System.Net.Http;
using Leaves.Utils;

namespace Leaves.Api.Services
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

        public async Task<string> AddEventAsync(AddCalendarEventContract eventContract)
        {
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
                return null;
            }

            var response = result.Response;

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return GetEventUriFromJson(
                await response.Content.ReadAsStringAsync()
            );
        }

        private string GetEventUriFromJson(string json)
        {
            try
            {
                var jsonObject = JObject.Parse(json);
                var eventUri = jsonObject.Value<string>("htmlLink");
                return eventUri;
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}

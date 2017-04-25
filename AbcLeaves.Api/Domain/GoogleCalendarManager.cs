using System;
using System.Threading.Tasks;
using AbcLeaves.Api.Services;
using AbcLeaves.Core;
using AutoMapper;

namespace AbcLeaves.Api.Domain
{
    public class GoogleCalendarManager : IGoogleCalendarManager
    {
        private readonly IMapper mapper;
        private readonly IUserManager userManager;
        private readonly IGoogleOAuthService googleAuthService;
        private readonly GoogleCalendarApiClient googleCalendarApiClient;

        public GoogleCalendarManager(
            IMapper mapper,
            IUserManager userManager,
            IGoogleOAuthService googleAuthService,
            GoogleCalendarApiClient googleCalendarApiClient)
        {
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }
            if (userManager == null)
            {
                throw new ArgumentNullException(nameof(userManager));
            }
            if (googleAuthService == null)
            {
                throw new ArgumentNullException(nameof(googleAuthService));
            }
            if (googleCalendarApiClient == null)
            {
                throw new ArgumentNullException(nameof(googleCalendarApiClient));
            }

            this.mapper = mapper;
            this.userManager = userManager;
            this.googleAuthService = googleAuthService;
            this.googleCalendarApiClient = googleCalendarApiClient;
        }

        public async Task<OperationResult> PublishUserEventAsync(UserEventPublishDto userEvent)
        {
            var userId = userEvent.UserId;
            if (String.IsNullOrEmpty(userId))
            {
                // todo: log
                return OperationResult.Fail(
                    "An error occurred when publishing an event to Google Calendar");
            }

            var userResult = await userManager.FindUserByIdAsync(userId);
            if (!userResult.Succeeded)
            {
                return OperationResult.FailFrom(userResult);
            }
            var user = userResult.User;
            var tokenResult = await userManager.GetRefreshTokenAsync(user);
            if (!tokenResult.Succeeded)
            {
                return OperationResult.FailFrom(tokenResult);
            }
            var refreshToken = tokenResult.Value;
            var exchangeResult = await googleAuthService.ExchangeRefreshToken(refreshToken);
            if (!exchangeResult.Succeeded)
            {
                return OperationResult.FailFrom(exchangeResult);
            }
            var accessToken = exchangeResult.AccessToken;

            var eventAddDto = mapper.Map<UserEventPublishDto, CalendarEventAddDto>(
                userEvent,
                opts => opts.AfterMap((src, dst) => dst.AccessToken = accessToken));
            var eventAddResult = await googleCalendarApiClient.AddEventAsync(eventAddDto);
            if (!eventAddResult.Succeeded)
            {
                return OperationResult.FailFrom(eventAddResult);
            }
            var eventUri = eventAddResult.Value;
            return OperationResult.Success(eventUri);
        }
    }
}

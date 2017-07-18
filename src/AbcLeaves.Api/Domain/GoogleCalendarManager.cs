using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using AbcLeaves.Api.Services;
using AbcLeaves.Core;
using AutoMapper;

namespace AbcLeaves.Api.Domain
{
    public class GoogleCalendarManager
    {
        private readonly IMapper mapper;
        private readonly UserManager userManager;
        private readonly GoogleOAuthClient googleAuthClient;
        private readonly GoogleCalendarClient googleCalendarClient;

        public GoogleCalendarManager(
            IMapper mapper,
            UserManager userManager,
            GoogleOAuthClient googleAuthClient,
            GoogleCalendarClient googleCalendarClient)
        {
            this.mapper = Throw.IfNull(mapper, nameof(mapper));
            this.userManager = Throw.IfNull(userManager, nameof(userManager));
            this.googleAuthClient = Throw.IfNull(googleAuthClient, nameof(googleAuthClient));
            this.googleCalendarClient = Throw.IfNull(googleCalendarClient, nameof(googleCalendarClient));
        }

        public async Task<StringResult> PublishUserEventAsync(UserEventPublishDto userEvent)
        {
            Throw.IfNull(userEvent, nameof(userEvent));

            var userId = userEvent.UserId;
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return StringResult.Fail(
                    "An error occurred when publishing an event to Google Calendar"
                );
            }

            var refreshToken = await userManager.GetRefreshTokenAsync(user);
            if (refreshToken == null)
            {
                return StringResult.Fail(
                    "An error occurred when publishing an event to Google Calendar"
                );
            }

            var exchangeResult = await googleAuthClient.ExchangeRefreshToken(refreshToken);
            if (!exchangeResult.Succeeded)
            {
                return StringResult.Fail(
                    "An error occurred when publishing an event to Google Calendar"
                );
            }

            var accessToken = exchangeResult.AccessToken;
            var eventAddDto = mapper.Map<UserEventPublishDto, CalendarEventAddDto>(
                userEvent,
                opts => opts.AfterMap((src, dst) => dst.AccessToken = accessToken)
            );
            var eventAddResult = await googleCalendarClient.AddEventAsync(eventAddDto);
            if (!eventAddResult.Succeeded)
            {
                return StringResult.Fail(
                    "An error occurred when publishing an event to Google Calendar"
                );
            }

            var eventUri = eventAddResult.Value;
            return StringResult.Succeed(eventUri);
        }

        public async Task<VerifyAccessResult> GrantAccess(
            string code,
            string redirectUrl,
            ClaimsPrincipal principal)
        {
            var user = await userManager.GetOrCreateUserAsync(principal);
            if (user == null)
            {
                return VerifyAccessResult.Fail(
                    "Failed to grant access to google apis"
                );
            }

            var exchangeCodeResult = await googleAuthClient.ExchangeAuthCode(code, redirectUrl);
            if (!exchangeCodeResult.Succeeded)
            {
                return VerifyAccessResult.Fail(
                    "Failed to grant access to google apis"
                );
            }

            var idToken = exchangeCodeResult.Tokens.IdToken;
            if (!VerifyOAuthExchangeIdentity())
            {
                return VerifyAccessResult.Fail(
                    "Failed to grant access to google apis. " +
                    "Authorization code doesn't match the authenticated identity"
                );
            }

            var refreshToken = exchangeCodeResult.Tokens.RefreshToken;
            var identityResult = await userManager.SetRefreshTokenAsync(user, refreshToken);
            if (!identityResult.Succeeded)
            {
                return VerifyAccessResult.Fail(
                    "Failed to grant access to google apis"
                );
            }

            return VerifyAccessResult.Succeed();

            bool VerifyOAuthExchangeIdentity()
            {
                var subject = userManager.GetSubjectClaim(principal);
                if (subject == null)
                {
                    return false;
                }

                var idTokenSubject = GetJwtSubject();
                if (idTokenSubject == null)
                {
                    return false;
                }

                if (!String.Equals(subject, idTokenSubject, StringComparison.Ordinal))
                {
                    return false;
                }

                return true;

                string GetJwtSubject()
                {
                    JwtSecurityToken jwtToken = null;
                    try
                    {
                        jwtToken = new JwtSecurityToken(idToken);
                    }
                    catch (ArgumentNullException)
                    {
                        return null;
                    }
                    catch (ArgumentException)
                    {
                        return null;
                    }
                    return jwtToken.Subject;
                }
            }
        }
    }
}

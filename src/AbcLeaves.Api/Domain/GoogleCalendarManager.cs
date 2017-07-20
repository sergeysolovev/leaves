using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using AbcLeaves.Api.Services;
using AbcLeaves.Utils;
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

        public async Task<string> PublishUserEventAsync(PublishUserEventContract eventContract)
        {
            Throw.IfNull(eventContract, nameof(eventContract));

            var userId = eventContract.UserId;
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            var refreshToken = await userManager.GetRefreshTokenAsync(user);
            if (refreshToken == null)
            {
                return null;
            }

            var accessToken = await googleAuthClient.ExchangeRefreshToken(refreshToken);
            if (String.IsNullOrEmpty(accessToken))
            {
                return null;
            }

            var addEventContract = mapper.Map<PublishUserEventContract, AddCalendarEventContract>(
                eventContract,
                opts => opts.AfterMap((src, dst) => dst.AccessToken = accessToken)
            );

            return await googleCalendarClient.AddEventAsync(addEventContract);
        }

        public async Task<GrantAccessResult> GrantAccess(
            string code,
            string redirectUrl,
            ClaimsPrincipal principal)
        {
            var user = await userManager.GetOrCreateUserAsync(principal);
            if (user == null)
            {
                return GrantAccessResult.Fail(
                    "Failed to grant access to google calendar"
                );
            }

            var exchangeResponse = await googleAuthClient.ExchangeAuthCode(code, redirectUrl);
            if (exchangeResponse == null)
            {
                return GrantAccessResult.Fail(
                    "Failed to grant access to google calendar"
                );
            }

            var idToken = exchangeResponse.IdToken;
            if (!VerifyOAuthExchangeIdentity())
            {
                return GrantAccessResult.Fail(
                    "Failed to grant access to google calendar. " +
                    "Authorization code doesn't match the authenticated identity"
                );
            }

            var refreshToken = exchangeResponse.RefreshToken;
            var identityResult = await userManager.SetRefreshTokenAsync(user, refreshToken);
            if (!identityResult.Succeeded)
            {
                return GrantAccessResult.Fail(
                    "Failed to grant access to google calendar"
                );
            }

            return GrantAccessResult.Succeed();

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

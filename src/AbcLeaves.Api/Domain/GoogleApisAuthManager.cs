using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using AbcLeaves.Api.Services;
using AbcLeaves.Core;

namespace AbcLeaves.Api.Domain
{
    public class GoogleApisAuthManager
    {
        private readonly UserManager userManager;
        private readonly GoogleOAuthClient googleAuthClient;

        public GoogleApisAuthManager(UserManager userManager, GoogleOAuthClient googleAuthClient)
        {
            this.userManager = Throw.IfNull(userManager, nameof(userManager));
            this.googleAuthClient = Throw.IfNull(googleAuthClient, nameof(googleAuthClient));
        }

        public async Task<VerifyAccessResult> VerifyAccess(ClaimsPrincipal principal)
        {
            var user = await userManager.GetOrCreateUserAsync(principal);
            if (user == null)
            {
                return VerifyAccessResult.Fail(
                    "Failed to verify access to google apis"
                );
            }

            var refreshToken = await userManager.GetRefreshTokenAsync(user);
            if (String.IsNullOrEmpty(refreshToken))
            {
                return VerifyAccessResult.Succeed(isForbidden: true);
            }

            return await googleAuthClient.ValidateRefreshTokenAsync(refreshToken);
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
            if (!VerifyOAuthExchangeIdentity(principal, idToken))
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
        }

        private bool VerifyOAuthExchangeIdentity(ClaimsPrincipal principal, string idToken)
        {
            var subject = userManager.GetSubjectClaim(principal);
            if (subject == null)
            {
                return false;
            }

            var idTokenSubject = GetJwtSubject(idToken);
            if (idTokenSubject == null)
            {
                return false;
            }

            if (!String.Equals(subject, idTokenSubject, StringComparison.Ordinal))
            {
                return false;
            }

            return true;
        }

        private string GetJwtSubject(string token)
        {
            JwtSecurityToken jwtToken = null;
            try
            {
                jwtToken = new JwtSecurityToken(token);
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

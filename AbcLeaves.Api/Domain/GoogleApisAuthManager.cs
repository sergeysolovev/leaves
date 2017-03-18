using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using AbcLeaves.Api.Services;

namespace AbcLeaves.Api.Domain
{
    public class GoogleApisAuthManager : IGoogleApisAuthManager
    {
        private readonly IUserManager userManager;
        private readonly IGoogleOAuthService googleAuthService;

        public GoogleApisAuthManager(
            IUserManager userManager,
            IGoogleOAuthService googleAuthService)
        {
            if (userManager == null)
            {
                throw new ArgumentNullException(nameof(userManager));
            }
            if (googleAuthService == null)
            {
                throw new ArgumentNullException(nameof(googleAuthService));
            }

            this.userManager = userManager;
            this.googleAuthService = googleAuthService;
        }

        public async Task<VerifyAccessResult> VerifyAccess(ClaimsPrincipal principal)
        {
            return await Operation<VerifyAccessResult>
                .BeginWith(() => userManager.EnsureUserCreatedAsync(principal))
                .ExitOnFailWith(user => VerifyAccessResult.FailFrom(user))
                .ProceedWith(user => userManager.GetRefreshTokenAsync(user.User))
                .ProceedWith(token => googleAuthService.ValidateRefreshTokenAsync(token.Value))
                .ExitOnFailWith(x => VerifyAccessResult.FailForbidden)
                .Return();
        }

        public async Task<VerifyAccessResult> GrantAccess(
            string code,
            string redirectUrl,
            ClaimsPrincipal principal)
        {
            return await Operation<VerifyAccessResult>
                .BeginWith(() => userManager.GetSubjectClaim(principal))
                .ProceedWithClosure(subject => subject
                    .ProceedWith(x => userManager.EnsureUserCreatedAsync(principal))
                    .ProceedWithClosure(user => user
                        .ProceedWith(x => googleAuthService.ExchangeAuthCode(code, redirectUrl))
                        .ProceedWithClosure(exchange => exchange
                            .ProceedWith(x => VerifyOAuthExchangeIdentity(
                                principal: principal,
                                idToken: exchange.Current.Tokens.IdToken))
                            .ProceedWith(x => userManager.SaveRefreshTokenAsync(
                                user: user.Current.User,
                                token: exchange.Current.Tokens.RefreshToken)))))
                .EndWith(VerifyAccessResult.Success)
                .Return();
        }

        private async Task<OperationResult> VerifyOAuthExchangeIdentity(
            ClaimsPrincipal principal,
            string idToken)
        {
            var error = "Authorization code doesn't match the authenticated identity";
            return await Operation<OperationResult>
                .BeginWith(() => userManager.GetSubjectClaim(principal))
                .ProceedWithClosure(subject => subject
                    .ProceedWith(x => GetJwtSubject(idToken))
                    .ProceedWithClosure(idTokenSubject => idTokenSubject
                        .ProceedWith(x =>
                            String.Equals(
                                a: subject.Current.Value,
                                b: idTokenSubject.Current.Value,
                                comparisonType: StringComparison.Ordinal) ?
                            OperationResult.Success() : OperationResult.Fail(error))))
                .Return();
        }

        private OperationResult GetJwtSubject(string token)
        {
            JwtSecurityToken jwtToken = null;
            try
            {
                jwtToken = new JwtSecurityToken(token);
            }
            catch (ArgumentNullException)
            {
                OperationResult.Fail("Failed to retrieve subject from jwt token");
            }
            catch (ArgumentException)
            {
                OperationResult.Fail("Failed to retrieve subject from jwt token");
            }
            return OperationResult.Success(jwtToken.Subject);
        }
    }
}

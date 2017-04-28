using System;
using System.Threading.Tasks;
using AbcLeaves.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;

namespace AbcLeaves.BasicMvcClient.Domain
{
    public class AuthenticationManager : IAuthenticationManager, IBearerTokenProvider
    {
        private readonly HttpContext httpContext;
        private readonly IDataProtectionProvider dataProtectionProvider;

        public AuthenticationManager(
            IHttpContextAccessor httpContextAccessor,
            IDataProtectionProvider dataProtectionProvider)
        {
            if (httpContextAccessor == null)
            {
                throw new ArgumentNullException(nameof(httpContextAccessor));
            }

            this.dataProtectionProvider = dataProtectionProvider;
            this.httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<AuthTokenResult> GetBearerToken()
        {
            return await GetIdTokenAsync();
        }

        public async Task<AuthPropertiesResult> GetAuthenticationPropertiesAsync()
        {
            var authContext = new AuthenticateContext("GoogleOpenIdConnect");
            await httpContext.Authentication.AuthenticateAsync(authContext);
            if (authContext.Principal == null || authContext.Properties == null)
            {
                return AuthPropertiesResult.Fail("Not authenticated");
            }
            var authProperties = new AuthenticationProperties(authContext.Properties);
            return AuthPropertiesResult.Success(authProperties);
        }

        public async Task<AuthTokenResult> GetIdTokenAsync()
        {
            return await OperationFlow<AuthTokenResult>
                .BeginWith(() => GetAuthenticationPropertiesAsync())
                .ProceedWith(getProps => {
                    var idToken = getProps.AuthProperties.GetTokenValue("id_token");
                    if (String.IsNullOrEmpty(idToken))
                    {
                        return AuthTokenResult.Fail(
                            "Failed to retrieve id_token from authenticated identity");
                    }
                    return AuthTokenResult.Success(idToken);
                })
                .Return();
        }

        public async Task<AuthPropertiesResult> TestCrossSiteRequestForgery(string state)
        {
            return await OperationFlow<AuthPropertiesResult>
                .BeginWith(() => UnprotectState(state))
                .ProceedWithClosure(unprotect => unprotect
                    .ProceedWith(x => GetIdTokenFromState(unprotect.Current.AuthProperties))
                    .ProceedWithClosure(idTokenState => idTokenState
                        .ProceedWith(x => GetIdTokenAsync())
                        .ProceedWithClosure(idTokenCookie => idTokenCookie
                        .ProceedWith(x => {
                            var fromState = idTokenState.Current.Token;
                            var fromCookie = idTokenCookie.Current.Token;
                            if (!String.Equals(fromState, fromCookie, StringComparison.Ordinal))
                            {
                                return AuthPropertiesResult.Fail(
                                    "id_token from cookies and state don't match");
                            }
                            return AuthPropertiesResult.Success(unprotect.Current.AuthProperties);
                        }))))
                .Return();
        }

        public OperationResult GetProtectedState(AuthenticationProperties authProperties)
        {
            var stateDataFormat = CreateStateDataFormat();
            var state = stateDataFormat.Protect(authProperties);
            return OperationResult.Success(state);
        }

        private AuthPropertiesResult UnprotectState(string state)
        {
            var stateDataFormat = CreateStateDataFormat();
            var authProperties = stateDataFormat.Unprotect(state);
            if (authProperties == null)
            {
                return AuthPropertiesResult.Fail(
                    $"The value of the parameter {nameof(state)} is invalid");
            }
            return AuthPropertiesResult.Success(authProperties);
        }

        private AuthTokenResult GetIdTokenFromState(AuthenticationProperties authProperties)
        {
            var idToken = authProperties.GetTokenValue("id_token");
            if (String.IsNullOrEmpty(idToken))
            {
                return AuthTokenResult.Fail("Failed to retrieve id_token from the state");
            }
            return AuthTokenResult.Success(idToken);
        }

        private PropertiesDataFormat CreateStateDataFormat()
        {
            var dataProtector = dataProtectionProvider.CreateProtector(GetType().FullName);
            return new PropertiesDataFormat(dataProtector);
        }
    }
}

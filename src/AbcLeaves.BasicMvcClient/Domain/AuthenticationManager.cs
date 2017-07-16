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
    public class AuthenticationManager
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

        public async Task<AuthenticationProperties> GetAuthenticationPropertiesAsync()
        {
            var authContext = new AuthenticateContext("GoogleOpenIdConnect");
            await httpContext.Authentication.AuthenticateAsync(authContext);
            return new AuthenticationProperties(authContext.Properties);
        }

        public async Task<string> GetIdTokenAsync()
        {
            var authProps = await GetAuthenticationPropertiesAsync();
            return authProps.GetTokenValue("id_token");
        }

        public async Task<AuthPropertiesResult> TestCrossSiteRequestForgery(string state)
        {
            var authProps = UnprotectState(state);
            var idTokenFromState = authProps.GetTokenValue("id_token");
            var idTokenFromCookies = await GetIdTokenAsync();
            if (!String.Equals(idTokenFromState, idTokenFromCookies, StringComparison.Ordinal))
            {
                return AuthPropertiesResult.Fail(
                    "id_token from cookies and state don't match"
                );
            }
            return AuthPropertiesResult.Success(authProps);
        }

        public string GetProtectedState(AuthenticationProperties authProperties)
        {
            var stateDataFormat = CreateStateDataFormat();
            return stateDataFormat.Protect(authProperties);
        }

        private AuthenticationProperties UnprotectState(string state)
        {
            var stateDataFormat = CreateStateDataFormat();
            return stateDataFormat.Unprotect(state);
        }

        private PropertiesDataFormat CreateStateDataFormat()
        {
            var dataProtector = dataProtectionProvider.CreateProtector(GetType().FullName);
            return new PropertiesDataFormat(dataProtector);
        }
    }
}

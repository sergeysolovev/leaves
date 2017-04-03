using System;
using System.Threading.Tasks;
using AbcLeaves.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;

namespace AbcLeaves.BasicMvcClient.Domain
{
    public class HttpContextBearerTokenProvider : IBearerTokenProvider
    {
        private readonly HttpContext httpContext;

        public HttpContextBearerTokenProvider(
            IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor == null)
            {
                throw new ArgumentNullException(nameof(httpContextAccessor));
            }

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
            return await Operation<AuthTokenResult>
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
    }
}

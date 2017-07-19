using System;
using System.Threading.Tasks;
using AbcLeaves.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features.Authentication;

namespace AbcLeaves.BasicMvcClient.Helpers
{
    public class AuthHelper
    {
        private readonly HttpContext httpContext;
        private readonly IDataProtectionProvider dataProtectionProvider;

        public AuthHelper(
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

        public async Task<string> GetIdTokenAsync()
        {
            var authResult = await httpContext.AuthenticateAsync();
            if (!authResult.Succeeded)
            {
                return null;
            }

            var authProps = authResult.Properties;
            return authProps.GetTokenValue("id_token");
        }

        public string ProtectState(AuthenticationProperties authProperties)
        {
            var stateDataFormat = CreateStateDataFormat();
            return stateDataFormat.Protect(authProperties);
        }

        public AuthenticationProperties UnprotectState(string state)
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

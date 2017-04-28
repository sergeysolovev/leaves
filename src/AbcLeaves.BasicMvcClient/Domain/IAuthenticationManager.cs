using System.Threading.Tasks;
using AbcLeaves.Core;
using Microsoft.AspNetCore.Http.Authentication;

namespace AbcLeaves.BasicMvcClient.Domain
{
    public interface IAuthenticationManager
    {
        Task<AuthPropertiesResult> GetAuthenticationPropertiesAsync();
        Task<AuthTokenResult> GetIdTokenAsync();
        // OAuth 10.12 CSRF
        Task<AuthPropertiesResult> TestCrossSiteRequestForgery(string state);
        OperationResult GetProtectedState(AuthenticationProperties authProperties);
    }
}

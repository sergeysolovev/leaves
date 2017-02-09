using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net;
using ABC.Leaves.Api.Authorization;

namespace ABC.Leaves.Api.Middleware
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate next;
        IAuthorizationService authorizationService;

        public AuthorizationMiddleware(RequestDelegate next,
            IAuthorizationService authorizationService)
        {
            this.next = next;
            this.authorizationService = authorizationService;
        }

        public async Task Invoke(HttpContext context)
        {
            string accessToken = GetOAuthAccessToken(context);
            if (accessToken != null)
            {
                var result = await authorizationService.AuthorizeAdminAsync(accessToken);
                if (!result.IsAuthorized)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync(result.Error?.DeveloperMessage);
                    return;
                }
                await next(context);
            }
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }

        private static string GetOAuthAccessToken(HttpContext context)
        {
            string accessToken = null;
            string bearerAuthHeader = context.Request
                .Headers["Authorization"]
                .FirstOrDefault(h => h.StartsWith("Bearer"));
            if (bearerAuthHeader != null)
            {
                accessToken = bearerAuthHeader.Substring("Bearer ".Length).Trim();
            }
            return accessToken;
        }
    }
}

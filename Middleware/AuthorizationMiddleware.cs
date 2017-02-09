using System.Linq;
using System.Threading.Tasks;
using ABC.Leaves.Api.Repositories;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using ABC.Leaves.Api.GoogleAuth;
using ABC.Leaves.Api.GoogleAuth.Dto;
using System.Net;

namespace ABC.Leaves.Api.Middleware
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IEmployeeRepository employeeRepository;
        private readonly IGoogleAuthService googleAuthService;

        public AuthorizationMiddleware(RequestDelegate next,
            IEmployeeRepository employeeRepository,
            IGoogleAuthService googleAuthService)
        {
            this.next = next;
            this.employeeRepository = employeeRepository;
            this.googleAuthService = googleAuthService;
        }

        public async Task Invoke(HttpContext context)
        {
            string accessToken = GetOAuthAccessToken(context);
            if (accessToken != null)
            {
                var result = await googleAuthService.GetAccessTokenInfoAsync(accessToken);
                if (result.Error != null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return;
                }
                var userEmail = result.Email;
                if (!employeeRepository.CheckUserIsAdmin(result.Email))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return;
                }
                await next(context);
            }
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

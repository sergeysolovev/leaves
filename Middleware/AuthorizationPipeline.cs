using Microsoft.AspNetCore.Builder;

namespace ABC.Leaves.Api.Middleware
{
    public class AuthorizationPipeline
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<AuthorizationMiddleware>();
        }
    }
}

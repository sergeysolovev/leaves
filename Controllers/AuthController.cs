using ABC.Leaves.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABC.Leaves.Api.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthenticationService service;
        private const string GoogleAuthRedirectUrlRouteName = "GoogleAuthRedirectUrl";
        private string GoogleAuthRedirectUrl => Url.RouteUrl(
            GoogleAuthRedirectUrlRouteName, null, Request.Scheme);

        public AuthController(IAuthenticationService service)
        {
            this.service = service;
        }

        [Route("google/url")]
        [HttpGet]
        public IActionResult GetGoogleAuthenticationUrl()
        {
            var url = service.GetGoogleAuthenticationUrl(GoogleAuthRedirectUrl);

            return Ok(url);
        }

        [HttpGet("accesstoken/{code?}", Name = GoogleAuthRedirectUrlRouteName)]
        public IActionResult GetAccessToken(string code)
        {
            var token = service.GetAccessToken(code, GoogleAuthRedirectUrl);

            return Ok(token);
        }
    }
}

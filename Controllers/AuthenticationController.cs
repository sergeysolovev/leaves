using ABC.Leaves.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABC.Leaves.Api.Controllers
{
    [Route("api/[controller]")]
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationService service;
        public AuthenticationController(IAuthenticationService service)
        {
            this.service = service;
        }

        [Route("google/url")]
        [HttpGet]
        public IActionResult GetGoogleAuthenticationUrl()
        {
            var url = service.GetGoogleAuthenticationUrl();

            return Ok(url);
        }

        [Route("accesstoken")]
        [HttpGet("{code}")]
        public IActionResult GetAccessToken(string code)
        {
            var token = service.GetAccessToken(code);

            return Ok(token);
        }
    }
}

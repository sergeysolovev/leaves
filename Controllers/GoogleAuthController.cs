using System.Threading.Tasks;
using ABC.Leaves.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABC.Leaves.Api.Controllers
{
    [Route("api/[controller]")]
    public class GoogleAuthController : Controller
    {
        private readonly IGoogleAuthService service;
        private const string AuthRedirectUrlRouteName = "GoogleAuthRedirectUrl";
        private string AuthRedirectUrl => Url.RouteUrl(AuthRedirectUrlRouteName, null, Request.Scheme);

        public GoogleAuthController(IGoogleAuthService service)
        {
            this.service = service;
        }

        // GET api/googleauth/url
        [HttpGet("url")]
        public IActionResult GetAuthUrl()
        {
            var url = service.GetAuthUrl(AuthRedirectUrl);
            return Ok(url);
        }

        // GET api/googleauth/accesstoken?code=4/kc0pD2Jvif6If6tdt61hFsLTtG2TuSnYNNxMQHESBXE
        [HttpGet("accesstoken/{code?}", Name = AuthRedirectUrlRouteName)]
        public async Task<IActionResult> GetAccessToken(string code)
        {
            return await service.GetAccessTokenAsync(code, AuthRedirectUrl);
        }
    }
}

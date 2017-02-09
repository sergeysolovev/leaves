using System.Threading.Tasks;
using ABC.Leaves.Api.GoogleAuth;
using ABC.Leaves.Api.GoogleAuth.Dto;
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
            var input = new GetAuthUrlInput { RedirectUrl = AuthRedirectUrl };
            var output = service.GetAuthUrl(input);
            return Ok(output.AuthUrl);
        }

        // GET api/googleauth/accesstoken?code=4/kc0pD2Jvif6If6tdt61hFsLTtG2TuSnYNNxMQHESBXE
        [HttpGet("accesstoken/{code?}", Name = AuthRedirectUrlRouteName)]
        public async Task<IActionResult> GetAccessToken(string code)
        {
            var input = new GetAccessTokenAsyncInput { Code = code, RedirectUrl = AuthRedirectUrl };
            var output = await service.GetAccessTokenAsync(input);
            if (output.Error != null)
            {
                return new NotFoundObjectResult(output.Error);
            }
            else
            {
                return new OkObjectResult(output.AccessToken);
            }
        }
    }
}

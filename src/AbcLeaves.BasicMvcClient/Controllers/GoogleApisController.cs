using System.Threading.Tasks;
using AbcLeaves.Core;
using AbcLeaves.BasicMvcClient.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AbcLeaves.BasicMvcClient.Controllers
{
    [Route("googleapis")]
    [Authorize(ActiveAuthenticationSchemes = "GoogleOpenIdConnect")]
    public class GoogleApisController : Controller
    {
        private readonly GoogleApisAuthManager googleManager;

        public GoogleApisController(GoogleApisAuthManager googleApisAuthManager)
        {
            this.googleManager = googleApisAuthManager;
        }

        // GET /googleapis/access
        [HttpGet("access")]
        public async Task<IActionResult> GrantAccess([FromQuery]string returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(GrantAccessAcceptCode), null, null, Request.Scheme);
            return await googleManager
                .GetChallengeUrl(redirectUrl, returnUrl)
                .ToMvcActionResultAsync();
        }

        // GET /googleapis/access/accept
        [HttpGet("access/accept")]
        public async Task<IActionResult> GrantAccessAcceptCode(
            [FromQuery]string code = null,
            [FromQuery]string state = null,
            [FromQuery]string error = null)
        {
            var redirectUrl = Url.Action(null, null, null, Request.Scheme);
            return await googleManager
                .HandleOAuthExchangeCode(code, state, error, redirectUrl)
                .ToMvcActionResultAsync();
        }
    }
}

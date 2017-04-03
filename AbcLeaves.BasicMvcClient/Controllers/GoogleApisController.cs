using System.Threading.Tasks;
using AbcLeaves.BasicMvcClient.Domain;
using AbcLeaves.Core.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AbcLeaves.BasicMvcClient.Controllers
{
    [Route("googleapis")]
    [Authorize(ActiveAuthenticationSchemes = "GoogleOpenIdConnect")]
    public class GoogleApisController : Controller
    {
        private readonly IMvcActionResultHelper mvcHelper;
        private readonly IGoogleApisAuthManager googleManager;

        public GoogleApisController(
            IGoogleApisAuthManager googleApisAuthManager,
            IMvcActionResultHelper mvcHelper)
        {
            this.mvcHelper = mvcHelper;
            this.googleManager = googleApisAuthManager;
        }

        // GET /googleapis/access
        [HttpGet("access")]
        public async Task<IActionResult> GrantAccess([FromQuery]string returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(GrantAccessAcceptCode), null, null, Request.Scheme);
            var urlResult = await googleManager.GetChallengeUrl(redirectUrl, returnUrl);
            return mvcHelper.FromOperationResult(urlResult);
        }

        // GET /googleapis/access/accept
        [HttpGet("access/accept")]
        public async Task<IActionResult> GrantAccessAcceptCode(
            [FromQuery]string code = null,
            [FromQuery]string state = null,
            [FromQuery]string error = null)
        {
            var redirectUrl = Url.Action(null, null, null, Request.Scheme);
            var urlResult = await googleManager.HandleOAuthExchangeCode(code, state, error, redirectUrl);
            return mvcHelper.FromOperationResult(urlResult);
        }
    }
}

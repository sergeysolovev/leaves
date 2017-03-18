using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AbcLeaves.Api.Domain;

namespace AbcLeaves.Api.Controllers
{
    [Route("api/googleapis")]
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    public class GoogleApisController : ControllerBase
    {
        private readonly IGoogleApisAuthManager googleApisAuthManager;

        public GoogleApisController(IGoogleApisAuthManager googleApisAuthManager)
        {
            this.googleApisAuthManager = googleApisAuthManager;
        }

        // GET /api/googleapis/
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await googleApisAuthManager.VerifyAccess(HttpContext.User);
            return FromOperationResult(result);
        }

        // PATCH /api/googleapis/
        [HttpPatch]
        public async Task<IActionResult> Patch(
            [FromQuery]string code,
            [FromQuery]string redirectUrl)
        {
            var result = await googleApisAuthManager.GrantAccess(code, redirectUrl, HttpContext.User);
            return FromOperationResult(result);
        }
    }
}

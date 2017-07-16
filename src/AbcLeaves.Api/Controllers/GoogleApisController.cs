using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AbcLeaves.Api.Domain;
using AbcLeaves.Core;

namespace AbcLeaves.Api.Controllers
{
    [Route("api/googleapis")]
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    public class GoogleApisController : Controller
    {
        private readonly GoogleCalendarManager googleCalManager;

        public GoogleApisController(GoogleCalendarManager googleCalManager)
        {
            this.googleCalManager = googleCalManager;
        }

        // GET /api/googleapis/
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await googleCalManager.VerifyAccess(HttpContext.User);
            return result.ToMvcActionResult();
        }

        // PATCH /api/googleapis/
        [HttpPatch]
        public async Task<IActionResult> Patch(
            [FromQuery]string code,
            [FromQuery]string redirectUrl)
        {
            var result = await googleCalManager.GrantAccess(code, redirectUrl, HttpContext.User);
            return result.ToMvcActionResult();
        }
    }
}

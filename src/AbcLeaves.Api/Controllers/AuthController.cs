using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AbcLeaves.Api.Domain;
using System.Net;

namespace AbcLeaves.Api.Controllers
{
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly GoogleCalendarManager googleCalManager;

        public AuthController(GoogleCalendarManager googleCalManager)
        {
            this.googleCalManager = googleCalManager;
        }

        // POST /api/auth/googlecal/
        [HttpPost("googlecal")]
        public async Task<IActionResult> GrantAccessToGoogleCalendar(
            [FromQuery]string code,
            [FromQuery]string redirectUrl)
        {
            var grantAccessResult = await googleCalManager.GrantAccess(
                code,
                redirectUrl,
                HttpContext.User
            );

            if (!grantAccessResult.Succeeded)
            {
                return BadRequest(grantAccessResult.Error);
            }

            if (grantAccessResult.Forbidden)
            {
                return StatusCode((int)HttpStatusCode.Forbidden);
            }

            return Ok();
        }
    }
}

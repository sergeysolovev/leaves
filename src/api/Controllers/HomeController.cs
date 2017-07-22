using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Leaves.Api.Domain;
using System.Net;
using Leaves.Api.DataContract;
using Leaves.Api.Common;
using Newtonsoft.Json;

namespace Leaves.Api.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        // GET /
        [HttpGet("")]
        public IActionResult Index()
        {
            var protocol = Request.Scheme;
            var apiUrls = new ApiUrlsContract
            {
                GoogleCalendarAuthorizationsUrl = Url.Action<AuthController>(
                    nameof(AuthController.GrantAccessToGoogleCalendar),
                    protocol
                ),
                LeavesUrl = Url.Action<LeavesController>(
                    nameof(LeavesController.Get),
                    protocol
                )
            };

            return Json(apiUrls);
        }
    }
}

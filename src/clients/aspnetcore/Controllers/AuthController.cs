using System;
using System.Net;
using System.Threading.Tasks;
using Leaves.Client.DataContracts;
using Leaves.Client.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Leaves.Client.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly AuthHelper authHelper;

        public AuthController(AuthHelper authHelper)
        {
            this.authHelper = authHelper;
        }

        // GET /auth/idtoken
        [HttpGet("idtoken")]
        [Authorize]
        public async Task<IActionResult> IdToken()
        {
            var idToken = await authHelper.GetIdTokenAsync();
            if (String.IsNullOrEmpty(idToken))
            {
                return BadRequest();
            }

            ViewData["id_token"] = idToken;

            return View();
        }
    }
}

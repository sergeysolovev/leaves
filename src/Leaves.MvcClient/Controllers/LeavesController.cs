using System;
using System.Net;
using System.Threading.Tasks;
using Leaves.MvcClient.DataContracts;
using Leaves.MvcClient.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Leaves.MvcClient.Controllers
{
    [Route("leaves")]
    [Authorize]
    public class LeavesController : Controller
    {
        private readonly ApiClient apiClient;
        private readonly AuthHelper authHelper;
        private readonly GoogleOAuthHelper googleOAuthHelper;

        public LeavesController(
            ApiClient apiClient,
            AuthHelper authHelper,
            GoogleOAuthHelper googleOAuthHelper)
        {
            this.authHelper = authHelper;
            this.apiClient = apiClient;
            this.googleOAuthHelper = googleOAuthHelper;
        }

        // GET /leaves
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(
                await apiClient.GetLeaves()
            );
        }

        // GET /leaves/all
        [HttpGet("all")]
        public async Task<IActionResult> IndexAll()
        {
            return View(
                await apiClient.GetAllLeaves()
            );
        }

        // GET /leaves/apply/{start}/{end}
        [HttpGet("apply/{start}/{end}")]
        public async Task<IActionResult> Create(DateTime start, DateTime end)
        {
            var leave = new CreateLeaveContract {
                Start = ConvertToUtc(start),
                End = ConvertToUtc(end)
            };

            var applyLeaveResult = await apiClient.ApplyLeaveAsync(leave);
            if (applyLeaveResult.Response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return BadRequest();
            }

            return RedirectToAction(nameof(Index));
        }

        // POST /leaves
        [HttpPost]
        public async Task<IActionResult> Create([FromForm]CreateLeaveContract leave)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            leave.Start = ConvertToUtc(leave.Start);
            leave.End = ConvertToUtc(leave.End);

            var apiResult = await apiClient.ApplyLeaveAsync(leave);
            if (apiResult.Response.StatusCode == HttpStatusCode.Forbidden)
            {
                var returnUrl = Url.Action(
                    action: nameof(Create),
                    values: new {
                        start = leave.Start.ToString("O"),
                        end = leave.End.ToString("O")
                    }
                );

                var redirectUrl = Url.Action(nameof(AcceptAccessCode), null, null, Request.Scheme);

                var authResult = await HttpContext.AuthenticateAsync();
                if (!authResult.Succeeded)
                {
                    return BadRequest("Failed to apply a leave");
                }

                var authProps = authResult.Properties;
                authProps.Items.Add("returnUrl", returnUrl);
                var state = authHelper.ProtectState(authProps);
                var challengeUrl = googleOAuthHelper.BuildChallengeUrl(redirectUrl, state);

                return Redirect(challengeUrl);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET /leaves/new
        [HttpGet("new")]
        public IActionResult New()
        {
            return View();
        }

        // GET /leaves/approve/{id}
        [HttpGet("approve/{id}")]
        public async Task<IActionResult> ApproveLeave(string id)
        {
            var result = await apiClient.ApproveLeaveAsync(id);
            return RedirectToAction(nameof(IndexAll));
        }

        // GET leaves/decline/{id}
        [HttpGet("decline/{id}")]
        public async Task<IActionResult> DeclineLeave(string id)
        {
            var result = await apiClient.DeclineLeaveAsync(id);
            return RedirectToAction(nameof(IndexAll));
        }

        // GET /leaves/auth/googlecal/
        [HttpGet("auth/googlecal")]
        public async Task<IActionResult> AcceptAccessCode(
            [FromQuery]string code = null,
            [FromQuery]string state = null,
            [FromQuery]string error = null)
        {
            if (error != null)
            {
                return BadRequest($"Failed to grant access to Google Calendar: {error}");
            }
            if (code == null)
            {
                return BadRequest("Failed to grant access to Google Calendar");
            }

            var authProps = authHelper.UnprotectState(state);
            if (authProps == null)
            {
                return BadRequest("Failed to grant access to Google Calendar");
            }

            // RFC 6749 10.12
            if (!await TestCrossSiteRequestForgery())
            {
                return BadRequest("Failed to grant access to Google Calendar");
            }

            var redirectUrl = Url.Action(null, null, null, Request.Scheme);
            var grantAccessResult = await apiClient.GrantAccessToGoogleCalendar(
                code,
                redirectUrl
            );

            if (!grantAccessResult.Response.IsSuccessStatusCode)
            {
                return BadRequest("Failed to grant access to Google Calendar");
            }

            var returnUrl = authProps.Items["returnUrl"];
            if (String.IsNullOrEmpty(returnUrl))
            {
                return BadRequest("Failed to grant access to Google Calendar");
            }

            return Redirect(returnUrl);

            async Task<bool> TestCrossSiteRequestForgery()
            {
                var idTokenFromState = authProps.GetTokenValue("id_token");
                var idTokenFromCookies = await authHelper.GetIdTokenAsync();

                return String.Equals(
                    idTokenFromState,
                    idTokenFromCookies,
                    StringComparison.Ordinal
                );
            }
        }

        private static DateTime ConvertToUtc(DateTime dateTime)
        {
            switch (dateTime.Kind)
            {
                case DateTimeKind.Unspecified:
                    return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                case DateTimeKind.Local:
                    return dateTime.ToUniversalTime();
                default:
                    return dateTime;
            }
        }
    }
}

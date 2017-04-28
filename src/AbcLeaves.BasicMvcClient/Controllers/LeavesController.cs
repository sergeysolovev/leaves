using System;
using System.Threading.Tasks;
using AbcLeaves.BasicMvcClient.Domain;
using AbcLeaves.Core.Helpers;
using AbcLeaves.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AbcLeaves.BasicMvcClient.Controllers
{
    [Route("leaves")]
    [Authorize(ActiveAuthenticationSchemes = "GoogleOpenIdConnect")]
    public class LeavesController : Controller
    {
        private readonly LeavesApiClient leavesApiClient;
        private readonly IAuthenticationManager authManager;
        private readonly IMvcActionResultHelper mvcHelper;

        public LeavesController(
            LeavesApiClient leavesApiClient,
            IMvcActionResultHelper mvcHelper,
            IAuthenticationManager authHelper)
        {
            this.authManager = authHelper;
            this.leavesApiClient = leavesApiClient;
            this.mvcHelper = mvcHelper;
        }

        // GET /leaves/approve/{id}
        [HttpGet("approve/{id}")]
        public async Task<IActionResult> ApproveLeave(string id)
        {
            var result = await leavesApiClient.ApproveLeaveAsync(id);
            return mvcHelper.FromOperationResult(result);
        }

        // GET leaves/decline/{id}
        [HttpGet("decline/{id}")]
        public async Task<IActionResult> DeclineLeave(string id)
        {
            var result = await leavesApiClient.DeclineLeaveAsync(id);
            return mvcHelper.FromOperationResult(result);
        }

        // todo: get rid of this
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

        // POST /leaves
        [HttpPost]
        public async Task<IActionResult> Create([FromForm]CreateLeaveContract leave)
        {
            if (ModelState.IsValid)
            {
                var verifyAccess = await leavesApiClient.VerifyGoogleApisAccess();
                if (verifyAccess.Succeeded)
                {
                    leave.Start = ConvertToUtc(leave.Start);
                    leave.End = ConvertToUtc(leave.End);
                    var result = await leavesApiClient.ApplyLeaveAsync(leave);
                    return Json(result);
                }
                return BadRequest(verifyAccess);
            }
            return BadRequest(ModelState);
        }

        // GET /leaves/new
        [HttpGet("new")]
        public async Task<IActionResult> New()
        {
            return await NewInternal(afterGranted: false);
        }

        // GET /leaves/new/aftergranted
        [HttpGet("new/aftergranted")]
        public async Task<IActionResult> NewAfterGranted()
        {
            return await NewInternal(afterGranted: true);
        }

        private async Task<IActionResult> NewInternal(bool afterGranted)
        {
            var verifyAccess = await leavesApiClient.VerifyGoogleApisAccess();
            if (verifyAccess.IsForbidden && !afterGranted)
            {
                var returnUrl = Url.Action(nameof(NewAfterGranted));
                var redirectUrl = Url.Action<GoogleApisController>(
                    action: nameof(GoogleApisController.GrantAccess),
                    values: new { returnUrl = returnUrl });
                return Redirect(redirectUrl);
            }
            if (verifyAccess.IsGranted)
            {
                return View(nameof(New));
            }
            return BadRequest(verifyAccess);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Leaves.Api.Domain;
using AutoMapper;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Leaves.Api.Controllers
{
    [Route("leaves")]
    public class LeavesController : Controller
    {
        private readonly IMapper mapper;
        private readonly UserManager userManager;
        private readonly LeavesManager leavesManager;

        public LeavesController(
            IMapper mapper,
            UserManager userManager,
            LeavesManager leavesManager)
        {
            this.userManager = userManager;
            this.leavesManager = leavesManager;
            this.mapper = mapper;
        }

        // GET /leaves
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var user = await userManager.GetOrCreateUserAsync(HttpContext.User);
            if (user == null)
            {
                return BadRequest();
            }

            return Json(
                await leavesManager.GetByUserId(user.Id)
            );
        }

        // GET /leaves/all
        [HttpGet("all")]
        [Authorize(Policy = "CanManageAllLeaves")]
        public async Task<IActionResult> GetAll()
        {
            return Json(
                await leavesManager.GetAll()
            );
        }

        // POST /leaves
        [HttpPost]
        [Authorize(Policy = "CanApplyLeaves")]
        public async Task<IActionResult> Post([FromBody]PostLeaveContract leaveContract)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await userManager.GetOrCreateUserAsync(HttpContext.User);
            if (user == null)
            {
                return BadRequest();
            }

            var applyLeaveContract = mapper.Map<PostLeaveContract, ApplyLeaveContract>(
                leaveContract, opts => opts.AfterMap((src, dst) => dst.UserId = user.Id)
            );

            return LeaveToActionResult(
                await leavesManager.ApplyAsync(applyLeaveContract)
            );
        }

        // PATCH /leaves/{id}/approve
        [HttpPatch("{id}/approve")]
        [Authorize(Policy = "CanManageAllLeaves")]
        public async Task<IActionResult> Approve([FromRoute]int id)
            => LeaveToActionResult(
                await leavesManager.ApproveAsync(id)
            );

        // PATCH /leaves/{id}/decline
        [HttpPatch("{id}/decline")]
        [Authorize(Policy = "CanManageAllLeaves")]
        public async Task<IActionResult> Decline([FromRoute]int id)
            => LeaveToActionResult(
                await leavesManager.DeclineAsync(id)
            );

        private IActionResult LeaveToActionResult(LeaveResult leaveResult)
        {
            if (!leaveResult.Succeeded)
            {
                return BadRequest(leaveResult.Error);
            }

            if (leaveResult.NotFound)
            {
                return NotFound();
            }

            return Json(
                mapper.Map<GetLeavesItemContract>(leaveResult.Leave)
            );
        }
    }
}

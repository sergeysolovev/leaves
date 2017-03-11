using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ABC.Leaves.Api.Domain;
using AutoMapper;
using ABC.Leaves.Api.Helpers;

namespace ABC.Leaves.Api.Controllers
{
    [Route("api/leaves")]
    public class LeavesController : Controller
    {
        private readonly IUserManager userManager;
        private readonly ILeavesManager leavesManager;
        private readonly IModelStateHelper modelStateHelper;
        private readonly IMapper mapper;

        public LeavesController(IUserManager userManager,
            ILeavesManager leavesManager,
            IModelStateHelper modelStateHelper,
            IMapper mapper)
        {
            this.userManager = userManager;
            this.leavesManager = leavesManager;
            this.modelStateHelper = modelStateHelper;
            this.mapper = mapper;
        }

        // POST api/leaves
        [HttpPost]
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> Post([FromBody]LeavePostDto leaveDto)
        {
            LeaveApplyResult result;
            if (!ModelState.IsValid)
            {
                result = LeaveApplyResult.Fail(
                    errorMessage: "One or more errors occurred during validation",
                    validationErrors: modelStateHelper.GetValidationErrors(ModelState));
                return BadRequest(result);
            }

            var principal = HttpContext.User;
            var userResult = await userManager.GetUserAsync(principal);
            if (!userResult.Succeeded)
            {
                result = LeaveApplyResult.FailFrom(userResult);
                return BadRequest(result);
            }
            var user = userResult.User;
            var leaveApplyDto = mapper.Map<LeavePostDto, LeaveApplyDto>(
                leaveDto, opts => opts.AfterMap((src, dst) => dst.UserId = user.Id)
            );
            result = await leavesManager.ApplyAsync(leaveApplyDto);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        // PATCH api/leaves/{id}/approve
        [HttpPatchAttribute("{id}/approve")]
        [Authorize(ActiveAuthenticationSchemes = "Bearer", Policy = "CanApproveLeaves")]
        public async Task<IActionResult> Approve([FromRoute]int id)
        {
            var result = await leavesManager.ApproveAsync(id);
            if (!result.Succeeded)
            {
                if (result.NotFound)
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }
            return Ok(result);
        }

        // PATCH api/leaves/{id}/decline
        [HttpPatchAttribute("{id}/decline")]
        [Authorize(ActiveAuthenticationSchemes = "Bearer", Policy = "CanApproveLeaves")]
        public async Task<IActionResult> Decline([FromRoute]int id)
        {
            var result = await leavesManager.DeclineAsync(id);
            if (!result.Succeeded)
            {
                if (result.NotFound)
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}

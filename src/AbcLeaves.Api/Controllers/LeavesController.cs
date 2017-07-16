using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AbcLeaves.Api.Domain;
using AutoMapper;
using AbcLeaves.Api.Helpers;
using AbcLeaves.Core;

namespace AbcLeaves.Api.Controllers
{
    [Route("api/leaves")]
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    public class LeavesController : Controller
    {
        private readonly IMapper mapper;
        private readonly UserManager userManager;
        private readonly LeavesManager leavesManager;
        private readonly ModelStateHelper modelHelper;

        public LeavesController(
            IMapper mapper,
            UserManager userManager,
            LeavesManager leavesManager,
            ModelStateHelper modelStateHelper)
        {
            this.userManager = userManager;
            this.leavesManager = leavesManager;
            this.modelHelper = modelStateHelper;
            this.mapper = mapper;
        }

        // POST api/leaves
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]LeavePostDto leaveDto)
        {
            if (!ModelState.IsValid)
            {
                return ModelValidationResult
                    .Fail(modelHelper.GetValidationErrors(ModelState))
                    .ToMvcActionResult();
            }

            var user = await userManager.GetOrCreateUserAsync(HttpContext.User);
            if (user == null)
            {
                return BadRequest();
            }

            var leaveApplyDto = mapper.Map<LeavePostDto, LeaveApplyDto>(
                leaveDto, opts => opts.AfterMap((src, dst) => dst.UserId = user.Id)
            );

            return await leavesManager
                .ApplyAsync(leaveApplyDto)
                .ToMvcActionResultAsync();
        }

        // PATCH api/leaves/{id}/approve
        [HttpPatchAttribute("{id}/approve")]
        [Authorize(Policy = "CanApproveLeaves")]
        public async Task<IActionResult> Approve([FromRoute]int id)
        {
            return await leavesManager
                .ApproveAsync(id)
                .ToMvcActionResultAsync();
        }

        // PATCH api/leaves/{id}/decline
        [HttpPatchAttribute("{id}/decline")]
        [Authorize(Policy = "CanDeclineLeaves")]
        public async Task<IActionResult> Decline([FromRoute]int id)
        {
            return await leavesManager
                .DeclineAsync(id)
                .ToMvcActionResultAsync();
        }
    }
}

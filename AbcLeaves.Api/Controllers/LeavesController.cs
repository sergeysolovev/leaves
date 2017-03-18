using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AbcLeaves.Api.Domain;
using AutoMapper;
using AbcLeaves.Api.Helpers;

namespace AbcLeaves.Api.Controllers
{
    [Route("api/leaves")]
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    public class LeavesController : ControllerBase
    {
        private readonly IUserManager userManager;
        private readonly ILeavesManager leavesManager;
        private readonly IModelStateHelper modelHelper;
        private readonly IMapper mapper;

        public LeavesController(IUserManager userManager,
            ILeavesManager leavesManager,
            IModelStateHelper modelStateHelper,
            IMapper mapper)
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
            var result = await Operation<LeaveApplyResult>
                .BeginWith(() => ModelState.IsValid ?
                    ModelValidationResult.Success :
                    ModelValidationResult.Fail(modelHelper.GetValidationErrors(ModelState)))
                .ProceedWith(x => userManager.EnsureUserCreatedAsync(HttpContext.User))
                .ProceedWith(userResult => {
                    var user = userResult.User;
                    var leaveApplyDto = mapper.Map<LeavePostDto, LeaveApplyDto>(
                        leaveDto, opts => opts.AfterMap((src, dst) => dst.UserId = user.Id));
                    return leavesManager.ApplyAsync(leaveApplyDto);
                })
                .Return();
            return FromOperationResult(result);
        }

        // PATCH api/leaves/{id}/approve
        [HttpPatchAttribute("{id}/approve")]
        [Authorize(Policy = "CanApproveLeaves")]
        public async Task<IActionResult> Approve([FromRoute]int id)
        {
            var result = await leavesManager.ApproveAsync(id);
            return FromOperationResult(result);
        }

        // PATCH api/leaves/{id}/decline
        [HttpPatchAttribute("{id}/decline")]
        [Authorize(Policy = "CanDeclineLeaves")]
        public async Task<IActionResult> Decline([FromRoute]int id)
        {
            var result = await leavesManager.DeclineAsync(id);
            return FromOperationResult(result);
        }
    }
}

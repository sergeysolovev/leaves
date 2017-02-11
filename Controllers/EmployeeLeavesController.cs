using ABC.Leaves.Api.Leaves;
using ABC.Leaves.Api.Leaves.Dto;
using Microsoft.AspNetCore.Mvc;
using ABC.Leaves.Api.Middleware;
using System.Threading.Tasks;

namespace ABC.Leaves.Api.Controllers
{
    [Route("api/employee/leave")]
    [ValidateModel]
    public class EmployeeLeavesController : Controller
    {
        private readonly IEmployeeLeavesService leavesService;

        public EmployeeLeavesController(IEmployeeLeavesService leavesService)
        {
            this.leavesService = leavesService;
        }

        // POST api/employee/leave
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]EmployeeLeaveDto employeeLeaveDto)
        {
            var applyResult = await leavesService.ApplyAsync(employeeLeaveDto);
            if (applyResult.Error != null)
            {
                return new BadRequestObjectResult(applyResult);
            }
            return new OkObjectResult(applyResult);
        }

        // PATCH api/employee/leave/{id}/approve
        [HttpPatchAttribute("{id}/approve")]
        [MiddlewareFilter(typeof(AuthorizationPipeline))]
        public async Task<IActionResult> Approve(int id)
        {
            var approveResult = await leavesService.ApproveAsync(id);
            if (approveResult.LeaveNotFound)
            {
                return new NotFoundObjectResult(approveResult);
            }
            if (approveResult.GoogleCalendarEventAdded)
            {
                return new OkObjectResult(approveResult);
            }
            return new BadRequestObjectResult(approveResult);
        }

        // PATCH api/employee/leave/{id}/decline
        [HttpPatchAttribute("{id}/decline")]
        [MiddlewareFilter(typeof(AuthorizationPipeline))]
        public async Task<IActionResult> Decline(int id)
        {
            var declineResult = await leavesService.DeclineAsync(id);
            if (declineResult.LeaveNotFound)
            {
                return new NotFoundObjectResult(declineResult);
            }
            if (declineResult.Declined)
            {
                return new OkObjectResult(declineResult);
            }
            return new BadRequestObjectResult(declineResult);
        }
    }
}

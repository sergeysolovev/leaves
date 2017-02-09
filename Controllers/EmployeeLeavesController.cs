using ABC.Leaves.Api.Services;
using ABC.Leaves.Api.Dto;
using Microsoft.AspNetCore.Mvc;
using ABC.Leaves.Api.Middleware;

namespace ABC.Leaves.Api.Controllers
{
    [Route("api/employee/leave")]
    public class EmployeeLeavesController : Controller
    {
        private readonly IEmployeeLeavesService service;

        public EmployeeLeavesController(IEmployeeLeavesService service)
        {
            this.service = service;
        }

        // POST api/employee/leave
        [HttpPost]
        public IActionResult Post([FromBody]EmployeeLeaveDto employeeLeaveDto)
        {
            return service.Apply(employeeLeaveDto);
        }

        // PATCH api/employee/leave/{id}/approve
        [HttpPatchAttribute("{id}/approve")]
        [MiddlewareFilter(typeof(AuthorizationPipeline))]
        public IActionResult Approve(int id)
        {
            return service.Approve(id);
        }

        // PATCH api/employee/leave/{id}/decline
        [HttpPatchAttribute("{id}/decline")]
        [MiddlewareFilter(typeof(AuthorizationPipeline))]
        public IActionResult Decline(int id)
        {
            return service.Decline(id);
        }
    }
}

using ABC.Leaves.Api.Services;
using ABC.Leaves.Api.Dto;
using Microsoft.AspNetCore.Mvc;

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

        [Route("apply")]
        [HttpPost]
        public IActionResult Apply([FromBody]EmployeeLeaveDto employeeLeaveDto)
        {
            return service.Apply(employeeLeaveDto);
        }

        [Route("{id}/approve")]
        [HttpPut]
        public IActionResult Approve(string id, [FromBody]string accessToken)
        {
            return service.Approve(id, accessToken);
        }

        [Route("{id}/decline")]
        [HttpPut]
        public IActionResult Decline(string id, [FromBody]string accessToken)
        {
            return service.Decline(id, accessToken);
        }
    }
}

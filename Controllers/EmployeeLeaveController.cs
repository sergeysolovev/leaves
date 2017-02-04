using ABC.Leaves.Api.Services;
using ABC.Leaves.Api.ViewModels;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace ABC.Leaves.Api.Controllers
{
    [Route("api/employee/leave")]
    public class EmployeeLeaveController : Controller
    {
        private readonly IEmployeeLeavesService employeeLeavesService;
        
        public EmployeeLeaveController(IEmployeeLeavesService employeeLeavesService)
        {
            this.employeeLeavesService = employeeLeavesService;
        }

        [Route("apply")]
        [HttpPost]
        public IActionResult Apply([FromBody]EmployeeLeaveViewModel employeeLeaveViewModel)
        {
            return employeeLeavesService.Apply(employeeLeaveViewModel);
        }

        [Route("{id}/approve")]
        [HttpPut]
        public IActionResult Approve(string id, [FromBody]string accessToken)
        {
            return employeeLeavesService.Approve(id, accessToken);
        }

        [Route("{id}/decline")]
        [HttpPut]
        public IActionResult Decline(string id, [FromBody]string accessToken)
        {
            return employeeLeavesService.Decline(id, accessToken);
        }
    }
}

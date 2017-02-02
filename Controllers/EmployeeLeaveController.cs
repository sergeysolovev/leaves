using ABC.Leaves.Api.Services;
using ABC.Leaves.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ABC.Leaves.Api.Controllers
{
    [Route("api/[controller]")]
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

        [Route("approve")]
        [HttpPost]
        public IActionResult Approve([FromBody]string employeeLeaveId)
        {
            return employeeLeavesService.Approve(employeeLeaveId);
        }

        [Route("decline")]
        [HttpPost]
        public IActionResult Decline([FromBody]string employeeLeaveId)
        {
            return employeeLeavesService.Decline(employeeLeaveId);
        }
    }
}

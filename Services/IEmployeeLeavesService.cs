using ABC.Leaves.Api.Dto;
using Microsoft.AspNetCore.Mvc;

namespace ABC.Leaves.Api.Services
{
    public interface IEmployeeLeavesService
    {
        IActionResult Apply(EmployeeLeaveDto EmployeeLeaveDto);
        IActionResult Approve(int id);
        IActionResult Decline(int id);
    }
}

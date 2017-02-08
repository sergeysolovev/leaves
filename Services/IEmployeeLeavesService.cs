using ABC.Leaves.Api.Dto;
using Microsoft.AspNetCore.Mvc;

namespace ABC.Leaves.Api.Services
{
    public interface IEmployeeLeavesService
    {
        IActionResult Apply(EmployeeLeaveDto EmployeeLeaveDto);
        IActionResult Approve(string id, string accessToken);
        IActionResult Decline(string id, string accessToken);
    }
}

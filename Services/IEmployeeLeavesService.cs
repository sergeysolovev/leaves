using ABC.Leaves.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ABC.Leaves.Api.Services
{
    public interface IEmployeeLeavesService
    {
        IActionResult Apply(EmployeeLeaveViewModel viewModel);
        IActionResult Approve(string id, string accessToken);
        IActionResult Decline(string id, string accessToken);
    }
}

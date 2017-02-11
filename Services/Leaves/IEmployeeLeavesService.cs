using System.Threading.Tasks;
using ABC.Leaves.Api.Leaves.Dto;

namespace ABC.Leaves.Api.Leaves
{
    public interface IEmployeeLeavesService
    {
        Task<ApplyLeaveResult> ApplyAsync(EmployeeLeaveDto leaveDto);
        Task<ApproveLeaveResult> ApproveAsync(int id);
        Task<DeclineLeaveResult> DeclineAsync(int id);
    }
}

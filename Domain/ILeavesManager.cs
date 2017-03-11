using System.Threading.Tasks;

namespace ABC.Leaves.Api.Domain
{
    public interface ILeavesManager
    {
        Task<LeaveApplyResult> ApplyAsync(LeaveApplyDto leaveDto);
        Task<LeaveApproveResult> ApproveAsync(int leaveId);
        Task<LeaveDeclineResult> DeclineAsync(int leaveId);
    }
}

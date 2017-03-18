using System.Threading.Tasks;

namespace AbcLeaves.Api.Domain
{
    public interface ILeavesManager
    {
        Task<LeaveApplyResult> ApplyAsync(LeaveApplyDto leaveDto);
        Task<LeaveApproveResult> ApproveAsync(int leaveId);
        Task<LeaveDeclineResult> DeclineAsync(int leaveId);
    }
}

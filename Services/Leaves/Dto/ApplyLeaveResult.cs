using ABC.Leaves.Api.Services.Dto;

namespace ABC.Leaves.Api.Leaves.Dto
{
    public class ApplyLeaveResult
    {
        public bool IsApplied { get; set; }
        public int LeaveId { get; set; }
        public ErrorDto Error { get; set; }
    }
}

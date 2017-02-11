using ABC.Leaves.Api.Services.Dto;

namespace ABC.Leaves.Api.Leaves.Dto
{
    public class DeclineLeaveResult
    {
        public bool Declined { get; set; }
        public bool LeaveNotFound { get; set; }
        public ErrorDto Error { get; set; }
    }
}

using ABC.Leaves.Api.Services.Dto;

namespace ABC.Leaves.Api.Leaves.Dto
{
    public class ApproveLeaveResult
    {
        public bool GoogleCalendarEventAdded { get; set; }
        public string GoogleCalendarEventUri { get; set; }
        public bool LeaveNotFound { get; set; }
        public ErrorDto Error { get; set; }
    }
}

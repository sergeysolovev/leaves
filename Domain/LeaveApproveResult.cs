using Newtonsoft.Json;

namespace ABC.Leaves.Api.Domain
{
    public class LeaveApproveResult : IOperationResult
    {
        public static LeaveApproveResult Success(OperationResult shareResult)
        {
            return new LeaveApproveResult {
                Succeeded = true,
                ShareGoogleCalendarResult = shareResult
            };
        }

        public static LeaveApproveResult FailNotFound(int leaveId)
        {
            return new LeaveApproveResult {
                NotFound = true,
                ErrorMessage = $"Employee leave id={leaveId} is not found"
            };
        }

        public static LeaveApproveResult Fail(string message)
        {
            return new LeaveApproveResult {
                ErrorMessage = message
            };
        }

        public bool Succeeded { get; protected set; }

        public string ErrorMessage { get; protected set; }

        public OperationResult ShareGoogleCalendarResult { get; protected set; }

        [JsonIgnore]
        public bool NotFound { get; protected set; }
    }
}
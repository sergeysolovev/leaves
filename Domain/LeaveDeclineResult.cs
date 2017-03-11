using Newtonsoft.Json;

namespace ABC.Leaves.Api.Domain
{
    public class LeaveDeclineResult : IOperationResult
    {
        public static LeaveDeclineResult Success()
        {
            return new LeaveDeclineResult {
                Succeeded = true
            };
        }

        public static LeaveDeclineResult FailNotFound(int leaveId)
        {
            return new LeaveDeclineResult {
                NotFound = true,
                ErrorMessage = $"Employee leave id={leaveId} is not found"
            };
        }

        public static LeaveDeclineResult Fail(string message)
        {
            return new LeaveDeclineResult {
                ErrorMessage = message
            };
        }

        public bool Succeeded { get; protected set; }
        public string ErrorMessage { get; protected set; }

        [JsonIgnore]
        public bool NotFound { get; protected set; }
    }
}
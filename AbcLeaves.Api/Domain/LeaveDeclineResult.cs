using Newtonsoft.Json;

namespace AbcLeaves.Api.Domain
{
    public class LeaveDeclineResult : OperationResultBase, INotFoundOperationResult
    {
        [JsonIgnore]
        public bool NotFound { get; protected set; }

        public LeaveDeclineResult(bool succeeded) : base(succeeded) {}
        protected LeaveDeclineResult(string error) : base(error, null) {}
        protected LeaveDeclineResult(IOperationResult result) : base(result) {}

        public static LeaveDeclineResult Success => new LeaveDeclineResult(true);

        public static LeaveDeclineResult Fail(string error) => new LeaveDeclineResult(error);

        public static LeaveDeclineResult FailNotFound(int leaveId)
        {
            var error = $"Employee leave id={leaveId} is not found";
            return new LeaveDeclineResult(error) { NotFound = true };
        }
    }
}
using AbcLeaves.Core;
using Newtonsoft.Json;

namespace AbcLeaves.Api.Domain
{
    public class LeaveDeclineResult : OperationResult, IFindResult
    {
        [JsonIgnore]
        public bool NotFound { get; protected set; }

        public LeaveDeclineResult() : base() { }
        protected LeaveDeclineResult(string error) : base(error) { }

        public static LeaveDeclineResult Success
            => new LeaveDeclineResult();

        public static LeaveDeclineResult Fail(string error)
            => new LeaveDeclineResult(error);

        public static LeaveDeclineResult FailNotFound(int leaveId)
        {
            var error = $"Employee leave id={leaveId} is not found";
            return new LeaveDeclineResult(error) { NotFound = true };
        }
    }
}

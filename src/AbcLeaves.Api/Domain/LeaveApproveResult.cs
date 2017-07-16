using System;
using AbcLeaves.Core;
using Newtonsoft.Json;

namespace AbcLeaves.Api.Domain
{
    public class LeaveApproveResult : OperationResult, IFindResult
    {
        [JsonIgnore]
        public bool NotFound { get; protected set; }
        public OperationResult ShareGoogleCalendarResult { get; private set; }

        protected LeaveApproveResult(OperationResult shareResult) : base()
        {
            ShareGoogleCalendarResult = Throw.IfNull(shareResult, nameof(shareResult));
        }

        protected LeaveApproveResult(string error) : base(error)
        {
        }

        public static LeaveApproveResult Success(OperationResult shareResult)
            => new LeaveApproveResult(shareResult);

        public static LeaveApproveResult Fail(string error)
            => new LeaveApproveResult(error);

        public static LeaveApproveResult FailNotFound(int leaveId)
        {
            var error = $"Employee leave id={leaveId} is not found";
            return new LeaveApproveResult(error) { NotFound = true };
        }
    }
}

using System;
using Newtonsoft.Json;

namespace AbcLeaves.Api.Domain
{
    public class LeaveApproveResult : OperationResultBase, INotFoundOperationResult
    {
        [JsonIgnore]
        public bool NotFound { get; protected set; }
        public OperationResult ShareGoogleCalendarResult { get; private set; }

        public LeaveApproveResult() : base()
        {
        }

        protected LeaveApproveResult(OperationResult shareResult)
        {
            if (shareResult == null)
            {
                throw new ArgumentNullException(nameof(shareResult));
            }

            ShareGoogleCalendarResult = shareResult;
        }

        protected LeaveApproveResult(string error)
            : base(error, null)
        {
        }

        protected LeaveApproveResult(IOperationResult result)
            : base(result)
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
using System;
using System.Collections.Generic;
using AbcLeaves.Api.Models;
using AbcLeaves.Api.Operations;
using Newtonsoft.Json;

namespace AbcLeaves.Api.Domain
{
    public class LeaveResult : OperationResult<Leave>, IFindResult
    {
        public Leave Leave => base.Value;

        public int? LeaveId => Leave?.Id;

        [JsonIgnore]
        public bool NotFound { get; protected set; }

        protected LeaveResult() : base() { }

        protected LeaveResult(Leave leave) : base(leave) { }

        protected LeaveResult(string error) : base(error) { }

        public static LeaveResult Succeed(Leave leave)
            => new LeaveResult(leave);

        public static LeaveResult Succeed()
            => new LeaveResult();

        public static LeaveResult Fail(string error)
            => new LeaveResult(error);

        public static LeaveResult FailNotFound(int leaveId)
        {
            var error = $"Leave id={leaveId} is not found";
            return new LeaveResult(error) { NotFound = true };
        }
    }
}

using System;
using System.Collections.Generic;
using AbcLeaves.Api.Models;
using AbcLeaves.Core;
using Newtonsoft.Json;

namespace AbcLeaves.Api.Domain
{
    public class LeaveApplyResult : OperationResultBase
    {
        [JsonIgnore]
        public Leave Leave { get; private set; }
        public int? LeaveId => Leave?.Id;

        public LeaveApplyResult() : base()
        {
        }

        protected LeaveApplyResult(Leave leave) : base(true)
        {
            if (leave == null)
            {
                throw new ArgumentNullException(nameof(leave));
            }
            Leave = leave;
        }

        protected LeaveApplyResult(string error, Dictionary<string, object> details)
            : base(error, details)
        {
        }

        protected LeaveApplyResult(IOperationResult result)
            : base(result)
        {
        }

        public static LeaveApplyResult Success(Leave leave) => new LeaveApplyResult(leave);

        public static LeaveApplyResult Fail(string error, Dictionary<string, object> details)
            => new LeaveApplyResult(error, details);

        public static LeaveApplyResult FailFrom(IOperationResult result)
            => new LeaveApplyResult(result);
    }
}

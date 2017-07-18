using System;
using System.Collections.Generic;
using AbcLeaves.Api.Models;
using AbcLeaves.Core;
using Newtonsoft.Json;

namespace AbcLeaves.Api.Domain
{
    public class LeaveApplyResult : OperationResult<Leave>
    {
        [JsonIgnore]
        public Leave Leave => base.Value;
        public int? LeaveId => Leave?.Id;

        protected LeaveApplyResult(Leave leave) : base(leave)
        {
        }

        protected LeaveApplyResult(string error) : base(error)
        {
        }

        public static LeaveApplyResult Success(Leave leave)
            => new LeaveApplyResult(leave);

        public static LeaveApplyResult Fail(string error)
            => new LeaveApplyResult(error);
    }
}

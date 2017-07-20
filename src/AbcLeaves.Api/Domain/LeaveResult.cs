using System;
using System.Collections.Generic;
using AbcLeaves.Api.Models;
using AbcLeaves.Utils;
using Newtonsoft.Json;

namespace AbcLeaves.Api.Domain
{
    public class LeaveResult
    {
        public bool Succeeded => (Error == null);
        public string Error { get; private set; }
        public Leave Leave { get; private set; }
        public bool NotFound { get; private set; }

        private LeaveResult(Leave leave = null)
            => Leave = leave;

        protected LeaveResult(string error)
            => Error = Throw.IfNullOrEmpty(error, nameof(error));

        public static LeaveResult Succeed(Leave leave)
            => new LeaveResult(leave);

        public static LeaveResult Succeed()
            => new LeaveResult();

        public static LeaveResult Fail(string error)
            => new LeaveResult(error);

        public static LeaveResult ReturnNotFound()
            => new LeaveResult() { NotFound = true };
    }
}

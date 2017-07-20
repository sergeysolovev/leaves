using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AbcLeaves.Api.Operations
{
    public class VerifyAccessResult : OperationResult<bool>, IVerifyAccessResult
    {
        public bool IsForbidden => base.Value;
        protected VerifyAccessResult(bool isForbidden) : base(isForbidden) { }
        protected VerifyAccessResult(Failure failure) : base(failure) { }
        protected VerifyAccessResult(string error) : base(error) { }

        public static VerifyAccessResult Succeed(bool isForbidden = false)
            => new VerifyAccessResult(isForbidden);

        public static VerifyAccessResult Fail(Failure failure)
            => new VerifyAccessResult(failure);

        public static VerifyAccessResult FailFrom(IOperationResult source)
            => Fail(source.Failure);

        public static VerifyAccessResult Fail(string error)
            => new VerifyAccessResult(error);
    }
}

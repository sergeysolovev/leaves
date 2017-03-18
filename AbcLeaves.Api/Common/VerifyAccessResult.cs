using System;
using System.Collections.Generic;

namespace AbcLeaves.Api
{
    public class VerifyAccessResult : OperationResultBase, IForbiddenOperationResult
    {
        public enum AccessType
        {
            Error,
            Forbidden,
            Granted
        }

        public AccessType Access { get; private set; }
        public static AccessType Forbidden => AccessType.Forbidden;
        public static AccessType Granted => AccessType.Granted;
        public static AccessType Error => AccessType.Error;
        public bool IsForbidden => (Access == Forbidden);

        public VerifyAccessResult() : base()
        {
        }

        protected VerifyAccessResult(AccessType accessType)
            : base(succeeded: accessType == Granted)
        {
            Access = accessType;
        }

        protected VerifyAccessResult(string error, Dictionary<string, object> details)
            : base(error, details)
        {
        }

        protected VerifyAccessResult(IOperationResult result)
            : base(result)
        {
        }

        public static VerifyAccessResult Success => new VerifyAccessResult(Granted);

        public static VerifyAccessResult FailForbidden => new VerifyAccessResult(Forbidden);

        public static VerifyAccessResult Fail(string error)
            => new VerifyAccessResult(error, null);

        public static VerifyAccessResult Fail(string error, Dictionary<string, object> details)
            => new VerifyAccessResult(error, details);

        public static VerifyAccessResult FailFrom(IOperationResult result)
            => new VerifyAccessResult(result);
    }
}

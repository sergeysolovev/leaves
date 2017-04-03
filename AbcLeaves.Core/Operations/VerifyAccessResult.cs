using System.Collections.Generic;
using Newtonsoft.Json;

namespace AbcLeaves.Core
{
    public class VerifyAccessResult : OperationResultBase, IForbiddenOperationResult
    {
        protected enum AccessTypeCode
        {
            Error,
            Forbidden,
            Granted
        }

        [JsonIgnore] protected AccessTypeCode AccessType { get; private set; }
        [JsonIgnore] public bool IsForbidden => (AccessType == AccessTypeCode.Forbidden);
        [JsonIgnore] public bool IsError => (AccessType == AccessTypeCode.Error);
        [JsonIgnore] public bool IsGranted => (AccessType == AccessTypeCode.Granted);
        public string Access => AccessType.ToString();

        public VerifyAccessResult() : base()
        {
        }

        protected VerifyAccessResult(AccessTypeCode accessType)
            : base(succeeded: accessType == AccessTypeCode.Granted)
        {
            AccessType = accessType;
        }

        protected VerifyAccessResult(string error, Dictionary<string, object> details)
            : base(error, details)
        {
        }

        protected VerifyAccessResult(IOperationResult result)
            : base(result)
        {
        }

        public static VerifyAccessResult Success
            => new VerifyAccessResult(AccessTypeCode.Granted);

        public static VerifyAccessResult FailForbidden
            => new VerifyAccessResult(AccessTypeCode.Forbidden);

        public static VerifyAccessResult Fail(string error)
            => new VerifyAccessResult(error, null);

        public static VerifyAccessResult Fail(string error, Dictionary<string, object> details)
            => new VerifyAccessResult(error, details);

        public static VerifyAccessResult FailFrom(IOperationResult result)
            => new VerifyAccessResult(result);
    }
}

using System.Collections.Generic;

namespace AbcLeaves.Api
{
    public class OperationResult : OperationResultBase
    {
        public string Value { get; private set; }

        public OperationResult() : base() {}
        protected OperationResult(bool succeeded) : base(succeeded) {}
        protected OperationResult(IOperationResult fromResult) : base(fromResult) {}

        protected OperationResult(string value)
            : base(true)
        {
            Value = value;
        }

        protected OperationResult(string error, Dictionary<string, object> details)
            : base(error, details)
        {
        }

        public static OperationResult Success() => new OperationResult(true);

        public static OperationResult Success(string value) => new OperationResult(value);

        public static OperationResult Fail(string error, Dictionary<string, object> details = null)
            => new OperationResult(error, details);

        public static OperationResult FailFrom(IOperationResult fromResult)
            => new OperationResult(fromResult);
    }
}

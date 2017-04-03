using System.Collections.Generic;
using AbcLeaves.Core;

namespace AbcLeaves.BasicMvcClient.Domain
{
    public class ReturnUrlResult : OperationResultBase, IReturnUrlOperationResult
    {
        public string ReturnUrl { get; private set; }

        public ReturnUrlResult() : base() {}
        protected ReturnUrlResult(bool succeeded) : base(succeeded) {}
        protected ReturnUrlResult(IOperationResult fromResult) : base(fromResult) {}

        protected ReturnUrlResult(string returnUrl)
            : base(true)
        {
            ReturnUrl = returnUrl;
        }

        protected ReturnUrlResult(string error, Dictionary<string, object> details)
            : base(error, details)
        {
        }

        public static ReturnUrlResult Success()
            => new ReturnUrlResult(true);

        public static ReturnUrlResult Success(string value)
            => new ReturnUrlResult(value);

        public static ReturnUrlResult Fail(string error, Dictionary<string, object> details = null)
            => new ReturnUrlResult(error, details);

        public static ReturnUrlResult FailFrom(IOperationResult fromResult)
            => new ReturnUrlResult(fromResult);
    }
}

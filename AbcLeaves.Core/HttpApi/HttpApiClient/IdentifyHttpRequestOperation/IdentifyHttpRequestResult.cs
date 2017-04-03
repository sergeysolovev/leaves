using System.Collections.Generic;
using System.Net.Http;

namespace AbcLeaves.Core
{
    public class IdentifyHttpRequestResult : OperationResultBase
    {
        public HttpRequestMessage Request { get; private set; }

        public IdentifyHttpRequestResult() : base() {}
        protected IdentifyHttpRequestResult(bool succeeded) : base(succeeded) {}
        protected IdentifyHttpRequestResult(IOperationResult fromResult) : base(fromResult) {}

        protected IdentifyHttpRequestResult(HttpRequestMessage identifiedRequest)
            : base(true)
        {
            Request = identifiedRequest;
        }

        protected IdentifyHttpRequestResult(string error, Dictionary<string, object> details)
            : base(error, details)
        {
        }

        public static IdentifyHttpRequestResult Success(HttpRequestMessage identifiedRequest)
            => new IdentifyHttpRequestResult(identifiedRequest);

        public static IdentifyHttpRequestResult Fail(string error, Dictionary<string, object> details = null)
            => new IdentifyHttpRequestResult(error, details);

        public static IdentifyHttpRequestResult FailFrom(IOperationResult fromResult)
            => new IdentifyHttpRequestResult(fromResult);
    }
}

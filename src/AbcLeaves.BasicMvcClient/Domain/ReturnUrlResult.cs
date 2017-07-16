using System;
using System.Collections.Generic;
using AbcLeaves.Core;

namespace AbcLeaves.BasicMvcClient.Domain
{
    public class ReturnUrlResult : OperationResult, IReturnUrlResult
    {
        public string ReturnUrl { get; private set; }

        protected ReturnUrlResult(string url) : base()
            => ReturnUrl = Throw.IfNullOrEmpty(url, nameof(url));

        protected ReturnUrlResult(Failure failure) : base(failure) { }

        public static ReturnUrlResult Succeed(string url)
            => new ReturnUrlResult(url);

        public static ReturnUrlResult Fail(string error)
            => new ReturnUrlResult(new Failure(error));
    }
}

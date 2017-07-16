using System;

namespace AbcLeaves.Core
{
  public class StringResult : OperationResult<string>
    {
        protected StringResult(string value) : base(value) { }
        protected StringResult(Failure failure) : base(failure) { }
        public new string Value => base.Value;
        public static StringResult Succeed(string value) => new StringResult(value);
        public static StringResult Fail(string errorMessage) => Fail(new Failure(errorMessage));
        public static StringResult Fail(Failure failure) => new StringResult(failure);
        public static StringResult FailFrom(IOperationResult source) => Fail(source.Failure);
    }
}

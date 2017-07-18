namespace AbcLeaves.Core
{
  public class BooleanResult : OperationResult<bool>
    {
        protected BooleanResult(bool value) : base(value) { }
        protected BooleanResult(Failure failure) : base(failure) { }
        public new bool Value => base.Value;
        public static BooleanResult Succeed(bool value) => new BooleanResult(value);
        public static BooleanResult Fail(string errorMessage) => Fail(new Failure(errorMessage));
        public static BooleanResult Fail(Failure failure) => new BooleanResult(failure);
    }
}

namespace AbcLeaves.Core
{
    public interface IOperationResult
    {
        bool Succeeded { get; }
        Failure Failure { get; }
    }

    public abstract class OperationResult : IOperationResult
    {
        public bool Succeeded => (Failure == null);
        public Failure Failure { get; private set; }
        protected OperationResult() { }
        protected OperationResult(Failure failure) => Failure = failure;
        protected OperationResult(string error) : this(new Failure(error)) { }
    }

    public abstract class OperationResult<T> : OperationResult
    {
        protected T Value { get; private set; }
        protected OperationResult(T value) => Value = value;
        protected OperationResult(Failure failure) : base(failure) { }
        protected OperationResult(string error) : base(error) { }
    }
}

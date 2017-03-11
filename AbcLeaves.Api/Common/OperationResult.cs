namespace ABC.Leaves.Api
{
    public class OperationResult : IOperationResult
    {
        public static OperationResult Success(object value)
        {
            return new OperationResult { Succeeded = true, Value = value };
        }

        public static OperationResult Fail(string message)
        {
            return new OperationResult { ErrorMessage = message };
        }

        public static OperationResult FailFrom(IOperationResult fromResult)
        {
            return Fail(fromResult.ErrorMessage);
        }

        public T GetValue<T>()
        {
            return (T)Value;
        }

        public object Value { get; protected set; }
        public bool Succeeded { get; protected set; }
        public string ErrorMessage { get; protected set; }
    }
}
